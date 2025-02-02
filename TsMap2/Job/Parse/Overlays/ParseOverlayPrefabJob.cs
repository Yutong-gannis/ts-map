using System;
using System.Drawing;
using Serilog;
using TsMap2.Helper;
using TsMap2.Model.Ts;
using TsMap2.Scs;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Job.Parse.Overlays {
    public class ParseOverlayPrefabJob : ThreadJob {
        protected override void Do() {
            Log.Information( "[Job][OverlayPrefab] Parsing..." );

            foreach ( ScsMapPrefabItem prefab in Store().Map.Prefabs ) {
                // Bitmap b          = prefab.Overlay?.GetBitmap();
                ScsNode originNode = Store().Map.GetNodeByUid( prefab.Nodes[ 0 ] );

                if ( !prefab.Valid || prefab.Hidden || prefab.Prefab.PrefabNodes == null ) continue;

                TsPrefabNode mapPointOrigin = prefab.Prefab.PrefabNodes[ prefab.Origin ];

                var rot = (float)( originNode.Rotation
                                   - Math.PI
                                   - Math.Atan2( mapPointOrigin.RotZ, mapPointOrigin.RotX )
                                   + Math.PI / 2 );

                float prefabStartX = originNode.X - mapPointOrigin.X;
                float prefabStartZ = originNode.Z - mapPointOrigin.Z;
                foreach ( TsSpawnPoint spawnPoint in prefab.Prefab.SpawnPoints ) {
                    ParseTrigger( prefab, prefabStartX, prefabStartZ, rot, originNode );
                    TsMapOverlayItem ov = GenerateMapItem( prefabStartX, prefabStartZ, rot, spawnPoint, originNode );

                    if ( ov != null )
                        AddMapItem( ov );

                    // if ( saveAsPNG && !File.Exists( Path.Combine( overlayPath, $"{overlayName}.png" ) ) )
                    // b.Save( Path.Combine( overlayPath, $"{overlayName}.png" ) );
                }
            }

            Log.Information( "[Job][OverlayPrefab] Fuel: {0}",          Store().Map.Overlays.Fuel.Count );
            Log.Information( "[Job][OverlayPrefab] Service: {0}",       Store().Map.Overlays.Service.Count );
            Log.Information( "[Job][OverlayPrefab] WeightStation: {0}", Store().Map.Overlays.WeightStation.Count );
            Log.Information( "[Job][OverlayPrefab] TruckDealer: {0}",   Store().Map.Overlays.TruckDealer.Count );
            Log.Information( "[Job][OverlayPrefab] Garage: {0}",        Store().Map.Overlays.Garage.Count );
            Log.Information( "[Job][OverlayPrefab] Recruitment: {0}",   Store().Map.Overlays.Recruitment.Count );
        }

        private TsMapOverlayItem GenerateMapItem( float prefabStartX, float prefabStartZ, float rot, TsSpawnPoint spawnPoint, ScsNode originNode ) {
            PointF newPoint = ScsRenderHelper.RotatePoint( prefabStartX + spawnPoint.X,
                                                           prefabStartZ + spawnPoint.Z, rot,
                                                           originNode.X, originNode.Z );

            string           overlayName;
            TsMapOverlayType type;

            switch ( spawnPoint.Type ) {
                case TsSpawnPointType.GasPos: {
                    overlayName = "gas_ico";
                    type        = TsMapOverlayType.Fuel;
                    break;
                }
                case TsSpawnPointType.ServicePos: {
                    overlayName = "service_ico";
                    type        = TsMapOverlayType.Service;
                    break;
                }
                case TsSpawnPointType.WeightStationPos: {
                    overlayName = "weigh_station_ico";
                    type        = TsMapOverlayType.WeightStation;
                    break;
                }
                case TsSpawnPointType.TruckDealerPos: {
                    overlayName = "dealer_ico";
                    type        = TsMapOverlayType.TruckDealer;
                    break;
                }
                case TsSpawnPointType.BuyPos: {
                    overlayName = "garage_large_ico";
                    type        = TsMapOverlayType.Garage;
                    break;
                }
                case TsSpawnPointType.RecruitmentPos: {
                    overlayName = "recruitment_ico";
                    type        = TsMapOverlayType.Recruitment;
                    break;
                }
                default:
                    return null;
            }

            TsMapOverlay overlay = Store().Def.GetOverlay( overlayName, ScsOverlayTypes.Map );
            Bitmap       b       = overlay.GetBitmap();

            return b == null
                       ? null
                       : new TsMapOverlayItem( newPoint.X, newPoint.Y, overlayName, type, b );
        }

        private void ParseTrigger( ScsMapPrefabItem prefab, float prefabStartX, float prefabStartZ, float rot, ScsNode originNode ) {
            int lastId = -1;
            foreach ( TsTriggerPoint triggerPoint in prefab.Prefab.TriggerPoints ) {
                TsMapOverlay overlay = Store().Def.GetOverlay( "parking_ico", ScsOverlayTypes.Map );
                Bitmap       b       = overlay.GetBitmap();

                if ( triggerPoint.TriggerId             == lastId
                     || b                               == null
                     || triggerPoint.TriggerActionToken != ScsTokenHelper.StringToToken( "hud_parking" ) ) continue;


                PointF newPoint = ScsRenderHelper.RotatePoint( prefabStartX + triggerPoint.X,
                                                               prefabStartZ + triggerPoint.Z, rot,
                                                               originNode.X, originNode.Z );

                lastId = (int)triggerPoint.TriggerId;
                var ov = new TsMapOverlayItem( newPoint.X, newPoint.Y, "parking_ico", TsMapOverlayType.Parking, b );
                Store().Map.Overlays.Parking.Add( ov );

                // if ( saveAsPNG && !File.Exists( Path.Combine( overlayPath, $"{overlayName}.png" ) ) )
                //     b.Save( Path.Combine( overlayPath, $"{overlayName}.png" ) );
            }
        }

        private void AddMapItem( TsMapOverlayItem item ) {
            switch ( item.OverlayType ) {
                case TsMapOverlayType.Fuel: {
                    Store().Map.Overlays.Fuel.Add( item );
                    break;
                }
                case TsMapOverlayType.Service: {
                    Store().Map.Overlays.Service.Add( item );
                    break;
                }
                case TsMapOverlayType.WeightStation: {
                    Store().Map.Overlays.WeightStation.Add( item );
                    break;
                }
                case TsMapOverlayType.TruckDealer: {
                    Store().Map.Overlays.TruckDealer.Add( item );
                    break;
                }
                case TsMapOverlayType.Garage: {
                    Store().Map.Overlays.Garage.Add( item );
                    break;
                }
                case TsMapOverlayType.Recruitment: {
                    Store().Map.Overlays.Recruitment.Add( item );
                    break;
                }
            }
        }
    }
}