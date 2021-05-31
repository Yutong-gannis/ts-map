﻿using System.IO;
using Serilog;

namespace TsMap2.Model.TsMapItem {
    public class TsMapBusStopItem : TsMapItem {
        public TsMapBusStopItem( TsSector sector, int startOffset ) : base( sector, startOffset ) {
            Valid = false;
            if ( Sector.Version < 836 || Sector.Version >= 847 )
                TsBusStopItem825( startOffset );
            else if ( Sector.Version >= 836 || Sector.Version < 847 )
                TsBusStopItem836( startOffset );
            else
                Log.Warning( $"Unknown base file version ({Sector.Version}) for item {Type} in file '{Path.GetFileName( Sector.FilePath )}' @ {startOffset}." );
        }

        public void TsBusStopItem825( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags

            fileOffset += 0x05       + 0x18; // 0x05(flags) + 0x18(city_name & padding & prefab_uid & node_uid)
            BlockSize  =  fileOffset - startOffset;
        }

        public void TsBusStopItem836( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags

            fileOffset += 0x05       + 0x1C; // 0x05(flags) + 0x18(city_name & padding & prefab_uid & node_uid & padding2)
            BlockSize  =  fileOffset - startOffset;
        }
    }
}