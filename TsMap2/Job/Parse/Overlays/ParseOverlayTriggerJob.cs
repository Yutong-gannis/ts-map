﻿using System.Drawing;
using Serilog;
using TsMap2.Model.Ts;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Job.Parse.Overlays {
    public class ParseOverlayTriggerJob : ThreadJob {
        protected override void Do() {
            Log.Information( "[Job][OverlayTrigger] Parsing..." );

            foreach ( ScsMapTriggerItem trigger in Store().Map.Triggers ) {
                Bitmap b = trigger.Overlay?.GetBitmap();

                if ( !trigger.Valid || trigger.Hidden || b == null ) continue;

                var ov = new TsMapOverlayItem( trigger.X, trigger.Z, trigger.OverlayName, TsMapOverlayType.Parking, b );
                Store().Map.Overlays.Parking.Add( ov );
            }

            Log.Information( "[Job][OverlayTrigger] Parking: {0}", Store().Map.Overlays.Parking.Count );
        }
    }
}