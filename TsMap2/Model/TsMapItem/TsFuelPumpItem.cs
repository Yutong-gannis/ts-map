﻿using System.IO;
using Serilog;
using TsMap2.Helper;

namespace TsMap2.Model.TsMapItem {
    public class TsFuelPumpItem : TsItem {
        public TsFuelPumpItem( TsSector sector, int startOffset ) : base( sector, startOffset ) {
            this.Valid = false;
            if ( this.Sector.Version < 855 )
                this.TsFuelPumpItem825( startOffset );
            else if ( this.Sector.Version >= 855 )
                this.TsFuelPumpItem855( startOffset );
            else
                Log.Warning(
                            $"Unknown base file version ({this.Sector.Version}) for item {this.Type} in file '{Path.GetFileName( this.Sector.FilePath )}' @ {startOffset}." );
        }

        public void TsFuelPumpItem825( int startOffset ) {
            int fileOffset = startOffset + 0x34; // Set position at start of flags
            fileOffset     += 0x05       + 0x10; // 0x05(flags) + 0x10(node_uid & prefab_uid)
            this.BlockSize =  fileOffset - startOffset;
        }

        public void TsFuelPumpItem855( int startOffset ) {
            int fileOffset      = startOffset + 0x34;                                                      // Set position at start of flags
            int subItemUidCount = MemoryHelper.ReadInt32( this.Sector.Stream, fileOffset += 0x05 + 0x10 ); // 0x05(flags) + 0x10(2 uids)
            fileOffset     += 0x04       + 0x08 * subItemUidCount;                                         // 0x04(subItemUidCount) + subItemUids
            this.BlockSize =  fileOffset - startOffset;
        }
    }
}