﻿using System;
using System.IO;
using Serilog;
using TsMap2.Helper;
using TsMap2.Scs;

namespace TsMap2.Model.TsMapItem {
    public class TsTriggerItem : TsItem {
        public TsTriggerItem( TsSector sector, int startOffset ) : base( sector, startOffset ) {
            this.Valid = true;
            if ( this.Sector.Version < 829 )
                this.TsTriggerItem825( startOffset );
            else if ( this.Sector.Version >= 829 && this.Sector.Version < 875 )
                this.TsTriggerItem829( startOffset );
            else if ( this.Sector.Version >= 875 )
                this.TsTriggerItem875( startOffset );
            else
                Log.Warning(
                            $"Unknown base file version ({this.Sector.Version}) for item {this.Type} in file '{Path.GetFileName( this.Sector.FilePath )}' @ {startOffset}." );
        }

        public string       OverlayName { get; private set; }
        public TsMapOverlay Overlay     { get; private set; }

        public void TsTriggerItem825( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags
            int dlcGuardCount = Store().Game.IsEts2()
                                    ? ScsConst.Ets2DlcGuardCount
                                    : ScsConst.AtsDlcGuardCount;
            this.Hidden = MemoryHelper.ReadInt8( this.Sector.Stream, fileOffset + 0x01 ) > dlcGuardCount;
            int nodeCount          = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x05 );                    // 0x05(flags)
            int tagCount           = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * nodeCount ); // 0x04(nodeCount) + nodeUids
            int triggerActionCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * tagCount );  // 0x04(tagCount) + tags
            fileOffset += 0x04;                                                                                           // cursor after triggerActionCount

            for ( var i = 0; i < triggerActionCount; i++ ) {
                ulong action = MemoryHelper.ReadUInt64( this.Sector.Stream, fileOffset );
                if ( action == ScsHash.StringToToken( "hud_parking" ) ) {
                    this.OverlayName = "parking_ico";
                    this.Overlay     = Store().Def.LookupOverlay( ScsHash.StringToToken( this.OverlayName ) );
                    if ( this.Overlay == null ) {
                        Console.WriteLine( "Could not find parking overlay" );
                        this.Valid = false;
                    }
                }

                int hasParameters = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x08 ); // 0x08(action)
                fileOffset += 0x04;                                                                   // set cursor after hasParameters
                if ( hasParameters == 1 ) {
                    int parametersLength = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );
                    fileOffset += 0x04 + 0x04 + parametersLength;    // 0x04(parametersLength) + 0x04(padding) + text(parametersLength * 0x01)
                } else if ( hasParameters == 3 ) fileOffset += 0x08; // 0x08 (m_some_uid)

                int targetTagCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );
                fileOffset += 0x04 + targetTagCount * 0x08; // 0x04(targetTagCount) + targetTags
            }

            fileOffset     += 0x18; // 0x18(range & reset_delay & reset_distance & min_speed & max_speed & flags2)
            this.BlockSize =  fileOffset - startOffset;
        }

        public void TsTriggerItem829( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags
            int dlcGuardCount = Store().Game.IsEts2()
                                    ? ScsConst.Ets2DlcGuardCount
                                    : ScsConst.AtsDlcGuardCount;
            this.Hidden = MemoryHelper.ReadInt8( this.Sector.Stream, fileOffset + 0x01 ) > dlcGuardCount;
            int tagCount  = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x05 );                   // 0x05(flags)
            int nodeCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * tagCount ); // 0x04(nodeCount) + tags

            int triggerActionCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * nodeCount ); // 0x04(nodeCount) + nodeUids
            fileOffset += 0x04;                                                                                           // cursor after triggerActionCount

            for ( var i = 0; i < triggerActionCount; i++ ) {
                ulong action = MemoryHelper.ReadUInt64( this.Sector.Stream, fileOffset );
                if ( action == ScsHash.StringToToken( "hud_parking" ) ) {
                    this.OverlayName = "parking_ico";
                    this.Overlay     = Store().Def.LookupOverlay( ScsHash.StringToToken( this.OverlayName ) );
                    if ( this.Overlay == null ) {
                        Console.WriteLine( "Could not find parking overlay" );
                        this.Valid = false;
                    }
                }

                int hasOverride                   = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x08 ); // 0x08(action)
                if ( hasOverride > 0 ) fileOffset += 0x04 * hasOverride;

                int hasParameters = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 ); // 0x04(hasOverride)
                fileOffset += 0x04;                                                                   // set cursor after hasParameters
                if ( hasParameters == 1 ) {
                    int parametersLength = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );
                    fileOffset += 0x04 + 0x04 + parametersLength;    // 0x04(parametersLength) + 0x04(padding) + text(parametersLength * 0x01)
                } else if ( hasParameters == 3 ) fileOffset += 0x08; // 0x08 (m_some_uid)

                int targetTagCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x08 ); // 0x08(unk/padding)
                fileOffset += 0x04 + targetTagCount * 0x08;                                            // 0x04(targetTagCount) + targetTags
            }

            fileOffset     += 0x18; // 0x18(range & reset_delay & reset_distance & min_speed & max_speed & flags2)
            this.BlockSize =  fileOffset - startOffset;
        }

        public void TsTriggerItem875( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags
            int dlcGuardCount = Store().Game.IsEts2()
                                    ? ScsConst.Ets2DlcGuardCount
                                    : ScsConst.AtsDlcGuardCount;
            this.Hidden = MemoryHelper.ReadInt8( this.Sector.Stream, fileOffset + 0x01 ) > dlcGuardCount;
            int tagCount  = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x05 );                   // 0x05(flags)
            int nodeCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * tagCount ); // 0x04(nodeCount) + tags

            int triggerActionCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x04 + 0x08 * nodeCount ); // 0x04(nodeCount) + nodeUids
            fileOffset += 0x04;                                                                                           // cursor after triggerActionCount

            for ( var i = 0; i < triggerActionCount; i++ ) {
                ulong action = MemoryHelper.ReadUInt64( this.Sector.Stream, fileOffset );
                if ( action == ScsHash.StringToToken( "hud_parking" ) ) {
                    this.OverlayName = "parking_ico";
                    this.Overlay     = Store().Def.LookupOverlay( ScsHash.StringToToken( this.OverlayName ) );
                    if ( this.Overlay == null ) {
                        Console.WriteLine( "Could not find parking overlay" );
                        this.Valid = false;
                    }
                }

                int hasOverride = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x08 ); // 0x08(action)
                fileOffset += 0x04;                                                                 // set cursor after hasOverride
                if ( hasOverride < 0 ) continue;
                fileOffset += 0x04 * hasOverride; // set cursor after override values

                int parameterCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );
                fileOffset += 0x04; // set cursor after parameterCount

                for ( var j = 0; j < parameterCount; j++ ) {
                    int paramLength = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );
                    fileOffset += 0x04 + 0x04 + paramLength; // 0x04(paramLength) + 0x04(padding) + (param)
                }

                int targetTagCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset );

                fileOffset += 0x04 + targetTagCount * 0x08 + 0x08; // 0x04(targetTagCount) + targetTags + 0x04(m_range & m_type)
            }

            if ( nodeCount == 1 ) fileOffset += 0x04; // 0x04(m_radius)
            this.BlockSize = fileOffset - startOffset;
        }
    }
}