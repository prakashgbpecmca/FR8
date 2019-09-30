angular.module('ngApp.common')
.factory("TopCurrencyService", function (DirectBookingService) {

    var topCurrencyOrder = function (CurrencyTypesStrore) {
        var curr = CurrencyTypesStrore;
        var CurrencyTypes = [];
        var cur1 = [];
        // Set Currency type 

        for (n = 0; n < curr.length; n++) {

            if (curr[n].CurrencyCode === "AUD") {
                CurrencyTypes.push({
                    CurrencyCode: curr[n].CurrencyCode,
                    CurrencyDescription: curr[n].CurrencyDescription,
                    OrderNumber: 4,
                    Disabled: false

                });
                curr.splice(n, 1);
                break;
            }

        }

        for (q = 0; q < curr.length; q++) {

            if (curr[q].CurrencyCode === "EUR") {
                CurrencyTypes.push({
                    CurrencyCode: curr[q].CurrencyCode,
                    CurrencyDescription: curr[q].CurrencyDescription,
                    OrderNumber: 7,
                    Disabled: false

                });
                curr.splice(q, 1);
                break;
            }

        }
        for (m = 0; m < curr.length; m++) {

            if (curr[m].CurrencyCode === "GBP") {
                CurrencyTypes.push({
                    CurrencyCode: curr[m].CurrencyCode,
                    CurrencyDescription: curr[m].CurrencyDescription,
                    OrderNumber: 3,
                    Disabled: false

                });
                curr.splice(m, 1);
                break;
            }

        }

        for (k = 0; k < curr.length; k++) {

            if (curr[k].CurrencyCode === "HKD") {
                CurrencyTypes.push({
                    CurrencyCode: curr[k].CurrencyCode,
                    CurrencyDescription: curr[k].CurrencyDescription,
                    OrderNumber: 1,
                    Disabled: false

                });
                curr.splice(k, 1);
                break;
            }

        }


        for (p = 0; p < curr.length; p++) {

            if (curr[p].CurrencyCode === "NZD") {
                CurrencyTypes.push({
                    CurrencyCode: curr[p].CurrencyCode,
                    CurrencyDescription: curr[p].CurrencyDescription,
                    OrderNumber: 6,
                    Disabled: false

                });
                curr.splice(p, 1);
                break;
            }

        }
        for (o = 0; o < curr.length; o++) {

            if (curr[o].CurrencyCode === "RMB") {
                CurrencyTypes.push({
                    CurrencyCode: curr[o].CurrencyCode,
                    CurrencyDescription: curr[o].CurrencyDescription,
                    OrderNumber: 5,
                    Disabled: false

                });
                curr.splice(o, 1);
                break;
            }

        } 
        for (mk = 0; mk < curr.length; mk++) {

            if (curr[mk].CurrencyCode === "THB") {
                CurrencyTypes.push({
                    CurrencyCode: curr[mk].CurrencyCode,
                    CurrencyDescription: curr[mk].CurrencyDescription,
                    OrderNumber: 8,
                    Disabled: false

                });
                curr.splice(mk, 1);
                break;
            }

        } 

        for (l = 0; l < curr.length; l++) {

            if (curr[l].CurrencyCode === "USD") {
                CurrencyTypes.push({
                    CurrencyCode: curr[l].CurrencyCode,
                    CurrencyDescription: curr[l].CurrencyDescription,
                    OrderNumber: 2,
                    Disabled: false

                });
                curr.splice(l, 1);
                break;
            }

        }
        
        for (r = 0; r < curr.length; r++) {

            CurrencyTypes.push({
                CurrencyCode: curr[r].CurrencyCode,
                CurrencyDescription: curr[r].CurrencyDescription,
                OrderNumber: r,
                Disabled: false

            });
            
        }

        for (s = 0; s < CurrencyTypes.length; s++) {

            cur1.push({
                CurrencyCode: CurrencyTypes[s].CurrencyCode,
                CurrencyDescription: CurrencyTypes[s].CurrencyDescription,
                OrderNumber: s,
                Disabled: false

            });

            if (s === 6) {
                cur1.push({
                    CurrencyCode: '  --------------------------',
                    CurrencyDescription: '-----------------------------  ',
                    
                    OrderNumber: s,
                    Disabled: true
                });
            }

        }

        return cur1;
    };

    return {
        TopCurrencyList: topCurrencyOrder
    };

});
 