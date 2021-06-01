﻿using System;
using Serilog;
using TsMap2.Helper;
using TsMap2.Job;
using TsMap2.Job.Export;
using TsMap2.Job.Export.Tiles;
using TsMap2.Job.Parse.Def;
using TsMap2.Job.Parse.Map;
using TsMap2.Job.Parse.Overlays;

namespace TsMap2 {
    internal class Program {
        private static void Main( string[] args ) {
            // Console.WriteLine( "===============================" );
            // Console.WriteLine( "> TsMap2 - &copy; 2021 JAGFx" );
            // Console.WriteLine( "> v0.0.0.0" );
            // Console.WriteLine( "===============================" );
            // Console.WriteLine( "" );

            LoggerHelper.Init();

            Log.Information( "HomeDir: {0}", AppPath.HomeDirApp );

            try {
                // -- Settings
                var s = new SettingsLoadJob();
                s.RunAndWait();

                // -- Parse
                var c = new ParseScsDefJob();
                c.Run();

                var m = new ParseMapJob();
                m.Run();

                var o = new ParseOverlaysJob();
                o.Run();

                // -- Export
                var e = new ExportJob();
                e.Run();

                var t = new ExportTilesJob();
                t.Run();

                // -- Debug
                // var d = new ExportDataOverviewJob();
                // d.RunAndWait();
            } catch ( Exception e ) {
                Log.Error( "Unexpected Exception: {0}", e.GetBaseException().ToString() );
                Log.Error( "Unexpected Exception: {0}", e.ToString() );
            }
        }
    }
}