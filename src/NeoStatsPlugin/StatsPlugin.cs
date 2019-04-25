﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Neo;
using Neo.Consensus;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Plugins;
using NeoStats.Core;
using NeoStats.Core.Extensions;
using NeoStatsPlugin.Extensions;

namespace NeoStatsPlugin
{
    public class StatsPlugin : Plugin, IPersistencePlugin, IP2PPlugin
    {
        private readonly BlockStatCollection _blocks = new BlockStatCollection();
        private long _P2PBytesReceived = 0;
        private long _P2PMsgReceived = 0;

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
            if (block != null)
            {
                block.StorageHash = "0x" + snapshot.Storages.GetChangeSet().Serialize().ToSha256().ToHexString().ToUpperInvariant();
                Log(block);
            }
        }

        public void OnPersist(Snapshot snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
        {
            var block = GetBlock(snapshot.PersistingBlock);

            if (block != null)
            {
                // Reset p2p stats

                block.P2P.Received.Bytes = Interlocked.Exchange(ref _P2PBytesReceived, 0);
                block.P2P.Received.Count = Interlocked.Exchange(ref _P2PMsgReceived, 0);
            }
        }

        #endregion

        #region P2P

        public bool OnP2PMessage(Message message)
        {
            // Increase p2p stats

            Interlocked.Add(ref _P2PBytesReceived, message.Size);
            Interlocked.Increment(ref _P2PMsgReceived);

            // Parse Message

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

                        var block = GetBlock(payload.BlockIndex);

                        switch (cnmsg)
                        {
                            case ChangeView view:
                                {
                                    //block?.OnChangeViewReceived(payload, view);
                                    break;
                                }
                            case PrepareRequest request:
                                {
                                    //block?.OnPrepareRequestReceived(payload, request);
                                    break;
                                }
                            case PrepareResponse response:
                                {
                                    //block?.OnPrepareResponseReceived(payload, response);
                                    break;
                                }
                            case Commit commit:
                                {
                                    //block?.OnCommitReceived(payload, commit);
                                    break;
                                }
                            case RecoveryMessage recovery:
                                {
                                    //block?.OnRecoveryMessageReceived(payload, recovery);
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
            if (!_blocks.Blocks.TryGetValue(index, out var ret))
            {
                ret = new BlockStat(index);
                _blocks.Blocks[index] = ret;

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

            if (!_blocks.Blocks.TryGetValue(block.Index, out var ret))
            {
                ret = new BlockStat(block.Index);
                _blocks.Blocks[block.Index] = ret;
            }

            // Update block info

            if (_blocks.Blocks.TryGetValue(block.Index - 1, out var prev))
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
            if (_blocks.Blocks.TryRemove(block.Index - 5, out var remove))
            {
                // Save 5 blocks
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