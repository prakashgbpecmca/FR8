angular.module('ngApp.common')
.factory("TopCountryService", function (DirectBookingService) {
    var topCountryOrder = function (CountriesStore) {
        var Countries = [];
        var CountriesRepo = [];
        var CountryData = CountriesStore;
        for (var p = 0; p < CountryData.length; p++) {
            if (CountryData[p].Code === "AUS") {
                Countries.push({
                    CountryId: CountryData[p].CountryId,
                    Name: CountryData[p].Name,
                    Code: CountryData[p].Code,
                    Code2: CountryData[p].Code2,
                    TimeZoneDetail: CountryData[p].TimeZoneDetail,
                    OrderNumber: 1
                });
                CountryData.splice(p, 1);
                break;
            }

        }
        for (var b = 0; b < CountryData.length; b++) {
            if (CountryData[b].Code === "CAN") {
                Countries.push({
                    CountryId: CountryData[b].CountryId,
                    Name: CountryData[b].Name,
                    Code: CountryData[b].Code,
                    Code2: CountryData[b].Code2,
                    TimeZoneDetail: CountryData[b].TimeZoneDetail,
                    OrderNumber: 2
                });
                CountryData.splice(b, 1);
                break;
            }

        }
        for (var c = 0; c < CountryData.length; c++) {
            if (CountryData[c].Code === "CHN") {
                Countries.push({
                    CountryId: CountryData[c].CountryId,
                    Name: CountryData[c].Name,
                    Code: CountryData[c].Code,
                    Code2: CountryData[c].Code2,
                    TimeZoneDetail: CountryData[c].TimeZoneDetail,
                    OrderNumber: 3
                });
                CountryData.splice(c, 1);
                break;
            }

        }
        //for (var d = 0; d < CountryData.length; d++) {
        //    if (CountryData[d].Code === "CH2") {
        //        Countries.push({
        //            CountryId: CountryData[d].CountryId,
        //            Name: CountryData[d].Name,
        //            Code: CountryData[d].Code,
        //            Code2: CountryData[d].Code2,
        //            TimeZoneDetail: CountryData[d].TimeZoneDetail,
        //            OrderNumber: 4
        //        });
        //        CountryData.splice(d, 1);
        //        break;
        //    }

        //}
        for (var e = 0; e < CountryData.length; e++) {
            if (CountryData[e].Code === "FRA") {
                Countries.push({
                    CountryId: CountryData[e].CountryId,
                    Name: CountryData[e].Name,
                    Code: CountryData[e].Code,
                    Code2: CountryData[e].Code2,
                    TimeZoneDetail: CountryData[e].TimeZoneDetail,
                    OrderNumber: 4
                });
                CountryData.splice(e, 1);
                break;
            }

        }
        for (var f = 0; f < CountryData.length; f++) {
            if (CountryData[f].Code === "DEU") {
                Countries.push({
                    CountryId: CountryData[f].CountryId,
                    Name: CountryData[f].Name,
                    Code: CountryData[f].Code,
                    Code2: CountryData[f].Code2,
                    TimeZoneDetail: CountryData[f].TimeZoneDetail,
                    OrderNumber: 5
                });
                CountryData.splice(f, 1);
                break;
            }

        }
        for (var g = 0; g < CountryData.length; g++) {
            if (CountryData[g].Code === "HKG") {
                Countries.push({
                    CountryId: CountryData[g].CountryId,
                    Name: CountryData[g].Name,
                    Code: CountryData[g].Code,
                    Code2: CountryData[g].Code2,
                    TimeZoneDetail: CountryData[g].TimeZoneDetail,
                    OrderNumber: 6
                });
                CountryData.splice(g, 1);
                break;
            }

        }
        for (var h = 0; h < CountryData.length; h++) {
            if (CountryData[h].Code === "IND") {
                Countries.push({
                    CountryId: CountryData[h].CountryId,
                    Name: CountryData[h].Name,
                    Code: CountryData[h].Code,
                    Code2: CountryData[h].Code2,
                    TimeZoneDetail: CountryData[h].TimeZoneDetail,
                    OrderNumber: 7
                });
                CountryData.splice(h, 1);
                break;
            }

        }
        for (var l = 0; l < CountryData.length; l++) {
            if (CountryData[l].Code === "THA") {
                Countries.push({
                    CountryId: CountryData[l].CountryId,
                    Name: CountryData[l].Name,
                    Code: CountryData[l].Code,
                    Code2: CountryData[l].Code2,
                    TimeZoneDetail: CountryData[l].TimeZoneDetail,
                    OrderNumber: 8
                });
                CountryData.splice(l, 1);
                break;
            }

        }
        for (var i = 0; i < CountryData.length; i++) {
            if (CountryData[i].Code === "GBR") {
                Countries.push({
                    CountryId: CountryData[i].CountryId,
                    Name: CountryData[i].Name,
                    Code: CountryData[i].Code,
                    Code2: CountryData[i].Code2,
                    TimeZoneDetail: CountryData[i].TimeZoneDetail,
                    OrderNumber: 9
                });
                CountryData.splice(i, 1);
                break;
            }

        }
        for (var j = 0; j < CountryData.length; j++) {
            if (CountryData[j].Code === "USA") {
                Countries.push({
                    CountryId: CountryData[j].CountryId,
                    Name: CountryData[j].Name,
                    Code: CountryData[j].Code,
                    Code2: CountryData[j].Code2,
                    TimeZoneDetail: CountryData[j].TimeZoneDetail,
                    OrderNumber: 10
                });
                CountryData.splice(j, 1);
                break;
            }

        }
        var z = 8;
        for (var k = 0; k < CountryData.length; k++) {
            Countries.push({
                CountryId: CountryData[k].CountryId,
                Name: CountryData[k].Name,
                Code: CountryData[k].Code,
                Code2: CountryData[k].Code2,
                TimeZoneDetail: CountryData[k].TimeZoneDetail,
                OrderNumber: k
            });
            z++;
        }
        for (var aa = 0; aa < Countries.length; aa++) {
            CountriesRepo.push({
                CountryId: Countries[aa].CountryId,
                Name: Countries[aa].Name,
                Code: Countries[aa].Code,
                Code2: Countries[aa].Code2,
                TimeZoneDetail: Countries[aa].TimeZoneDetail,
                OrderNumber: aa,
                Disabled: false
            });
            if (aa === 9) {
                CountriesRepo.push({
                    CountryId: "",
                    Name: '  -----------------------------------------------  ',
                    Code: '  -----------------------------------------------  ',
                    Code2: '  -----------------------------------------------  ',
                    TimeZoneDetail: null,
                    OrderNumber: aa,
                    Disabled: true
                });
            }
        }

        return CountriesRepo;
    };

    return {
        TopCountryList: topCountryOrder
    };

});


