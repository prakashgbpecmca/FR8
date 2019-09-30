angular.module('ngApp.common')
.factory("TopAirlineService", function (DirectBookingService) {

    function compare(a, b) {
        if (a.AirLineName < b.AirLineName) {
            return -1;
        }
        if (a.AirLineName > b.AirLineName) {
            return 1;
        }
        return 0;
    }



    var topAirlineOrder = function (AirlineStore) {
        var Airlines = [];
        var AirlineRepo = [];
        var AirlineData = AirlineStore;

        for (var p = 0; p < AirlineData.length; p++) {
            if (AirlineData[p].AilineCode === "698") {
                Airlines.push({
                    AirlineId: AirlineData[p].AirlineId,
                    AirLineName: AirlineData[p].AirLineName,
                    AilineCode: AirlineData[p].AilineCode,
                    CarrierCode2: AirlineData[p].CarrierCode2,
                    CarrierCode3: AirlineData[p].CarrierCode3,
                    OrderNumber: 1
                });
                AirlineData.splice(p, 1);
                break;
            }
        }
        for (var b = 0; b < AirlineData.length; b++) {
            if (AirlineData[b].AilineCode === "549") {
                Airlines.push({
                    AirlineId: AirlineData[b].AirlineId,
                    AirLineName: AirlineData[b].AirLineName,
                    AilineCode: AirlineData[b].AilineCode,
                    CarrierCode2: AirlineData[b].CarrierCode2,
                    CarrierCode3: AirlineData[b].CarrierCode3,
                    OrderNumber: 2
                });
                AirlineData.splice(b, 1);
                break;
            }

        }
        for (var c = 0; c < AirlineData.length; c++) {
            if (AirlineData[c].AilineCode === "125") {
                Airlines.push({
                    AirlineId: AirlineData[c].AirlineId,
                    AirLineName: AirlineData[c].AirLineName,
                    AilineCode: AirlineData[c].AilineCode,
                    CarrierCode2: AirlineData[c].CarrierCode2,
                    CarrierCode3: AirlineData[c].CarrierCode3,
                    OrderNumber: 3
                });
                AirlineData.splice(c, 1);
                break;
            }

        }

        //for (var d = 0; d < AirlineData.length; d++) {
        //    if (AirlineData[d].Code === "CH2") {
        //        Countries.push({
        //            CountryId: AirlineData[d].CountryId,
        //            Name: AirlineData[d].Name,
        //            Code: AirlineData[d].Code,
        //            Code2: AirlineData[d].Code2,
        //            TimeZoneDetail: AirlineData[d].TimeZoneDetail,
        //            OrderNumber: 4
        //        });
        //        AirlineData.splice(d, 1);
        //        break;
        //    } 
        //}

        for (var e = 0; e < AirlineData.length; e++) {
            if (AirlineData[e].AilineCode === "160") {
                Airlines.push({
                    AirlineId: AirlineData[e].AirlineId,
                    AirLineName: AirlineData[e].AirLineName,
                    AilineCode: AirlineData[e].AilineCode,
                    CarrierCode2: AirlineData[e].CarrierCode2,
                    CarrierCode3: AirlineData[e].CarrierCode3,
                    OrderNumber: 4
                });
                AirlineData.splice(e, 1);
                break;
            }

        }
        for (var f = 0; f < AirlineData.length; f++) {
            if (AirlineData[f].AilineCode === "176") {
                Airlines.push({
                    AirlineId: AirlineData[f].AirlineId,
                    AirLineName: AirlineData[f].AirLineName,
                    AilineCode: AirlineData[f].AilineCode,
                    CarrierCode2: AirlineData[f].CarrierCode2,
                    CarrierCode3: AirlineData[f].CarrierCode3,
                    OrderNumber: 5
                });
                AirlineData.splice(f, 1);
                break;
            }

        }
        for (var g = 0; g < AirlineData.length; g++) {
            if (AirlineData[g].AilineCode === "157") {
                Airlines.push({
                    AirlineId: AirlineData[g].AirlineId,
                    AirLineName: AirlineData[g].AirLineName,
                    AilineCode: AirlineData[g].AilineCode,
                    CarrierCode2: AirlineData[g].CarrierCode2,
                    CarrierCode3: AirlineData[g].CarrierCode3,
                    OrderNumber: 6
                });
                AirlineData.splice(g, 1);
                break;
            }

        }
        for (var h = 0; h < AirlineData.length; h++) {
            if (AirlineData[h].AilineCode === "618") {
                Airlines.push({
                    AirlineId: AirlineData[h].AirlineId,
                    AirLineName: AirlineData[h].AirLineName,
                    AilineCode: AirlineData[h].AilineCode,
                    CarrierCode2: AirlineData[h].CarrierCode2,
                    CarrierCode3: AirlineData[h].CarrierCode3,
                    OrderNumber: 7
                });
                AirlineData.splice(h, 1);
                break;
            }

        }
        for (var i = 0; i < AirlineData.length; i++) { 
            if (AirlineData[i].AilineCode === "235") {
                Airlines.push({
                    AirlineId: AirlineData[i].AirlineId,
                    AirLineName: AirlineData[i].AirLineName,
                    AilineCode: AirlineData[i].AilineCode,
                    CarrierCode2: AirlineData[i].CarrierCode2,
                    CarrierCode3: AirlineData[i].CarrierCode3,
                    OrderNumber: 8
                });
                AirlineData.splice(i, 1);
                break;
            }

        }

        //for (var j = 0; j < AirlineData.length; j++) {
        //    if (AirlineData[j].AilineCode === "AUS") {
        //        Airlines.push({
        //            AirlineId: AirlineData[j].AirlineId,
        //            AirLineName: AirlineData[j].AirLineName,
        //            AilineCode: AirlineData[j].AilineCode,
        //            CarrierCode2: AirlineData[j].CarrierCode2,
        //            CarrierCode3: AirlineData[j].CarrierCode3,
        //            OrderNumber: 9
        //        });
        //        AirlineData.splice(j, 1);
        //        break;
        //    } 
        //}

        var z = 7;

        for (var k = 0; k < AirlineData.sort(compare).length; k++) {

            Airlines.push({
                AirlineId: AirlineData[k].AirlineId,
                AirLineName: AirlineData[k].AirLineName,
                AilineCode: AirlineData[k].AilineCode,
                CarrierCode2: AirlineData[k].CarrierCode2,
                CarrierCode3: AirlineData[k].CarrierCode3,
                OrderNumber: k
            });

            z++;
        }
        for (var aa = 0; aa < Airlines.length; aa++) {
            AirlineRepo.push({
                AirlineId: Airlines[aa].AirlineId,
                AirLineName: Airlines[aa].AirLineName,
                AilineCode: Airlines[aa].AilineCode,
                CarrierCode2: Airlines[aa].CarrierCode2,
                CarrierCode3: Airlines[aa].CarrierCode3,
                OrderNumber: aa,
                Disabled: false
            });
            if (aa === 7) {
                AirlineRepo.push({
                    AirlineId: "",
                    AirLineName: '-----------------------------',
                    AilineCode: '-----------------------------',
                    CarrierCode2: '',
                    CarrierCode3: null,
                    OrderNumber: aa,
                    Disabled: true
                });
            }
        }




        return AirlineRepo;
    };

    return {

        TopAirlineList: topAirlineOrder

    };

});


