using System.Collections.Generic;
using Newtonsoft.Json;

namespace TsMap2.Model.Ts;

public class TsCity {
    public int   CountryId;
    public float X;
    public float Y;


    public TsCity( string name, string country, ulong token, string localizationToken, List< int > xOffsets, List< int > yOffsets ) {
        Name              = name;
        CountryName       = country;
        Token             = token;
        LocalizationToken = localizationToken;
        XOffsets          = xOffsets;
        YOffsets          = yOffsets;
    }

    public                string      Name              { get; set; }
    [ JsonIgnore ] public string      LocalizationToken { get; set; }
    public                string      CountryName       { get; set; }
    [ JsonIgnore ] public ulong       Token             { get; set; }
    [ JsonIgnore ] public List< int > XOffsets          { get; }
    [ JsonIgnore ] public List< int > YOffsets          { get; }

    public Dictionary< string, string > LocalizedNames { get; } = new Dictionary< string, string >();

    public void AddLocalizedName( string locale, string name ) {
        if ( !LocalizedNames.ContainsKey( locale ) ) LocalizedNames.Add( locale, name );
    }

    public string GetLocalizedName( string locale ) =>
        LocalizedNames.ContainsKey( locale )
            ? LocalizedNames[ locale ]
            : Name;
}