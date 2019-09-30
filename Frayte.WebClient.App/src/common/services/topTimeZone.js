angular.module('ngApp.common')
.factory("TopTimeZoneService", function (DirectBookingService) {
    var topTimeZoneOrder = function (TimeZonesStore) {
        var TimeZones = [];
        var FinalTimeZoneList = [];
        var TimeZoneData = TimeZonesStore;
        for (var p = 0; p < TimeZoneData.length; p++) {
            if (TimeZoneData[p].Code === "AUS") {
                TimeZones.push({
                    Name: TimeZoneData[p].TimeZoneDetail.Name,
                    Offset: TimeZoneData[p].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[p].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[p].TimeZoneDetail.TimezoneId,
                    OrderNumber: 1
                });
                TimeZoneData.splice(p, 1);
                break;
            }

        }
        for (var b = 0; b < TimeZoneData.length; b++) {
            if (TimeZoneData[b].Code === "CAN") {
                TimeZones.push({
                    Name: TimeZoneData[b].TimeZoneDetail.Name,
                    Offset: TimeZoneData[b].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[b].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[b].TimeZoneDetail.TimezoneId,
                    OrderNumber: 2
                });
                TimeZoneData.splice(b, 1);
                break;
            }

        }
        for (var c = 0; c < TimeZoneData.length; c++) {
            if (TimeZoneData[c].Code === "CHN") {
                TimeZones.push({
                    Name: TimeZoneData[c].TimeZoneDetail.Name,
                    Offset: TimeZoneData[c].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[c].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[c].TimeZoneDetail.TimezoneId,
                    OrderNumber: 3
                });
                TimeZoneData.splice(c, 1);
                break;
            }

        }
        for (var d = 0; d < TimeZoneData.length; d++) {
            if (TimeZoneData[d].Code === "CH2") {
                TimeZones.push({
                    Name: TimeZoneData[d].TimeZoneDetail.Name,
                    Offset: TimeZoneData[d].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[d].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[d].TimeZoneDetail.TimezoneId,
                    OrderNumber: 4
                });
                TimeZoneData.splice(d, 1);
                break;
            }

        }
        for (var e = 0; e < TimeZoneData.length; e++) {
            if (TimeZoneData[e].Code === "FRA") {
                TimeZones.push({
                    Name: TimeZoneData[e].TimeZoneDetail.Name,
                    Offset: TimeZoneData[e].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[e].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[e].TimeZoneDetail.TimezoneId,
                    OrderNumber: 5
                });
                TimeZoneData.splice(e, 1);
                break;
            }

        }
        for (var f = 0; f < TimeZoneData.length; f++) {
            if (TimeZoneData[f].Code === "DEU") {
                TimeZones.push({
                    Name: TimeZoneData[f].TimeZoneDetail.Name,
                    Offset: TimeZoneData[f].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[f].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[f].TimeZoneDetail.TimezoneId,
                    OrderNumber: 6
                });
                TimeZoneData.splice(f, 1);
                break;
            }

        }
        for (var g = 0; g < TimeZoneData.length; g++) {
            if (TimeZoneData[g].Code === "HKG") {
                TimeZones.push({
                    Name: TimeZoneData[g].TimeZoneDetail.Name,
                    Offset: TimeZoneData[g].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[g].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[g].TimeZoneDetail.TimezoneId,
                    OrderNumber: 7
                });
                TimeZoneData.splice(g, 1);
                break;
            }

        }
        for (var h = 0; h < TimeZoneData.length; h++) {
            if (TimeZoneData[h].Code === "IND") {
                TimeZones.push({
                    Name: TimeZoneData[h].TimeZoneDetail.Name,
                    Offset: TimeZoneData[h].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[h].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[h].TimeZoneDetail.TimezoneId,
                    OrderNumber: 8
                });
                TimeZoneData.splice(h, 1);
                break;
            }

        }
        for (var i = 0; i < TimeZoneData.length; i++) {
            if (TimeZoneData[i].Code === "GBR") {
                TimeZones.push({
                    Name: TimeZoneData[i].TimeZoneDetail.Name,
                    Offset: TimeZoneData[i].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[i].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[i].TimeZoneDetail.TimezoneId,
                    OrderNumber: 9
                });
                TimeZoneData.splice(i, 1);
                break;
            }

        }
        for (var j = 0; j < TimeZoneData.length; j++) {
            if (TimeZoneData[j].Code === "USA") {
                TimeZones.push({
                    Name: TimeZoneData[j].TimeZoneDetail.Name,
                    Offset: TimeZoneData[j].TimeZoneDetail.Offset,
                    OffsetShort: TimeZoneData[j].TimeZoneDetail.OffsetShort,
                    TimezoneId: TimeZoneData[j].TimeZoneDetail.TimezoneId,
                    OrderNumber: 10
                });
                TimeZoneData.splice(j, 1);
                break;
            }

        }
        var z = 9;
        for (var k = 0; k < TimeZoneData.length; k++) {
            TimeZones.push({
                Name: TimeZoneData[k].TimeZoneDetail.Name,
                Offset: TimeZoneData[k].TimeZoneDetail.Offset,
                OffsetShort: TimeZoneData[k].TimeZoneDetail.OffsetShort,
                TimezoneId: TimeZoneData[k].TimeZoneDetail.TimezoneId,
                OrderNumber: k
            });
            z++;
        }
        for (var aa = 0; aa < TimeZones.length; aa++) {
            if (TimeZones[aa].TimezoneId !== 0)
                {
                FinalTimeZoneList.push({
                    Name: TimeZones[aa].Name,
                    Offset: TimeZones[aa].Offset,
                    OffsetShort: TimeZones[aa].OffsetShort,
                    TimezoneId: TimeZones[aa].TimezoneId,
                    OrderNumber: aa,
                    Disabled: false
                });
            if (aa === 8) {
                FinalTimeZoneList.push({
                    
                    Name: ' ------------------------------',
                    Offset: '',
                    OffsetShort: '--------------------------',
                    TimezoneId: null,
                    OrderNumber: aa,
                    Disabled: true
                });
            }
        }
        }

        return FinalTimeZoneList;
    };

    return {
        TopTimeZoneOrder: topTimeZoneOrder
    };

});


