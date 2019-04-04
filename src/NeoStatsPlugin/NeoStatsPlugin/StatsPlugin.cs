using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Neo.Consensus;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Plugins;
using NeoStatsPlugin.Stats;

namespace NeoStatsPlugin
{
    public class StatsPlugin : Plugin, IPersistencePlugin, IP2PPlugin
    {
        private readonly ConcurrentDictionary<uint, BlockStat> _blocks = new ConcurrentDictionary<uint, BlockStat>();

        public override string Name => "StatsPlugin";

        public override void Configure()
        {
            Settings.Load(GetConfiguration());

            // Write empty json

            var dir = Path.GetDirectoryName(Settings.Default.Path);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(Settings.Default.Path, "[\n\n]");
        }

        #region Storage

        public void OnCommit(Snapshot snapshot)
        {
            var block = GetBlock(snapshot.PersistingBlock);

            if (block != null) Log(block);
        }

        public void OnPersist(Snapshot snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList) { }

        #endregion

        #region P2P

        public bool OnP2PMessage(Message message)
        {
            switch (message.Command)
            {
                case "block":
                    {
                        GetBlock(message.GetPayload<Block>());
                        break;
                    }
                case "consensus":
                    {
                        ConsensusPayload payload;
                        ConsensusMessage cnmsg;

                        try
                        {
                            payload = message.GetPayload<ConsensusPayload>();
                            cnmsg = payload.ConsensusMessage;
                        }
                        catch
                        {
                            break;
                        }

                        switch (cnmsg)
                        {
                            case ChangeView view:
                                {
                                    GetBlock(payload.BlockIndex)?.OnChangeViewReceived(payload, view);
                                    break;
                                }
                            case PrepareRequest request:
                                {
                                    GetBlock(payload.BlockIndex)?.OnPrepareRequestReceived(payload, request);
                                    break;
                                }
                            case PrepareResponse response:
                                {
                                    GetBlock(payload.BlockIndex)?.OnPrepareResponseReceived(payload, response);
                                    break;
                                }
                            case Commit commit:
                                {
                                    GetBlock(payload.BlockIndex)?.OnCommitReceived(payload, commit);
                                    break;
                                }
                            case RecoveryMessage recovery:
                                {
                                    GetBlock(payload.BlockIndex)?.OnRecoveryMessageReceived(payload, recovery);
                                    break;
                                }
                        }
                        break;
                    }
            }

            return true;
        }

        public bool OnConsensusMessage(ConsensusPayload payload) => true;

        #endregion

        /// <summary>
        /// Get block
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Get block</returns>
        public BlockStat GetBlock(uint index)
        {
            if (!_blocks.TryGetValue(index, out var ret))
            {
                ret = new BlockStat(index);
                _blocks[index] = ret;

                return ret;
            }

            return ret;
        }

        /// <summary>
        /// Get block
        /// </summary>
        /// <param name="block">Block</param>
        /// <returns>Get block</returns>
        public BlockStat GetBlock(Block block)
        {
            // Find

            if (!_blocks.TryGetValue(block.Index, out var ret))
            {
                ret = new BlockStat(block.Index);
                _blocks[block.Index] = ret;
            }

            // Update block info

            if (_blocks.TryGetValue(block.Index - 1, out var prev))
            {
                ret.UpdateBlockInfo(block, prev);
            }
            else
            {
                ret.UpdateBlockInfo(block, null);
            }

            return ret;
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="block">Block</param>
        public void Log(BlockStat block)
        {
            if (_blocks.TryRemove(block.Index - 5, out var remove))
            {
                // Save 5 blocks

                remove.Dispose();
            }

            using (var stream = File.OpenWrite(Settings.Default.Path))
            {
                stream.Seek(Math.Max(0, stream.Length - 2), SeekOrigin.Begin);

                var str = block.ToJson() + "\n]";

                if (stream.Position > 2)
                {
                    str = ",\n" + str;
                }

                var data = Encoding.ASCII.GetBytes(str);

                stream.Write(data, 0, data.Length);
            }
        }

        public bool ShouldThrowExceptionFromCommit(Exception ex) => false;
    }
}