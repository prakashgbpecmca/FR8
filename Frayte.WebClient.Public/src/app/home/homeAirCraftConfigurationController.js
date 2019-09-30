
angular.module('ngApp.home').controller('homeAirCraftConfigurationController', function ($scope, $uibModal, $location, $log, $filter) {
    $scope.ComingSoon = 'Coming soon...';
    //$scope.headerImage = 'Air_Freight_description01.png';

    $scope.toggleAirCraft = function () {
        if ($scope.search && $scope.airCraft.airline) {
            var found = $filter('filter')($scope.airCraft.airline, { standard: $scope.search });
            if (found.length) {
                return false;
            }
            else {
                return true;
            }
        }
        return false;
    };

    $scope.airCraft = {
        airline: [
            {
                modelno:'A380',
            standard: "7 PMC & 20 AKE",
            forward_hold: "22 AKE OR 7 PMC",
            aft_hold: "12 AKE OR 3 PMC",
            cargo_volume: "66 mc",
            uld_combination: "Yes on both hold",
            doorsizes: [
               { description: "Forward hold", inches: "67''x122''", centimetres: "172 cm x 311 cm" },
                 { description: "Aft hold", inches: "67''x109''", centimetres: "172 cm x 279 cm" },
                   { description: "Bulk", inches: "44''x55''", centimetres: "113 cm x 140 cm" }
            ],
            cargo_tonnage: "11100kgs (excluding the passenger and baggage load)",
            maximum_permitted_height: "64''",
            image: "Air_Freight_description01.png"
        },
                  {
                      modelno: 'A350',
                      standard: "6 PMC & 18 AKE",
                      forward_hold: "18 AKE OR 6 PMC",
                      aft_hold: "20 AKE OR 5 PMC",
                      cargo_volume: "96 mc",
                      uld_combination: "Yes on both hold",
                      doorsizes: [
                         { description: "Forward hold", inches: "69''x114''", centimetres: "176 cm x 291 cm" },
                           { description: "Aft hold", inches: "68''x112''", centimetres: "175 cm x 286 cm" },
                             { description: "Bulk", inches: "29''x37''", centimetres: "76 cm x 95 cm" }
                      ],
                      cargo_tonnage: "16400kgs (excluding the passenger and baggage load)",
                      maximum_permitted_height: "64''",
                      image: "Air_Freight_description02.png"
                  },
                  {
                      modelno: 'A340-600',
                      standard: "8 PMC & 18 AKE",
                      forward_hold: "4 PMC & 12 AKE",
                      aft_hold: "4 PMC & 6 AKE",
                      cargo_volume: "96 mc",
                      bulk_load: "Crew rest compartment",
                      uld_combination: "",
                      doorsizes: [
                         { description: "Forward hold", inches: "66''x106''", centimetres: "170 cm x 270 cm" },
                           { description: "Aft hold", inches: "66''x107''", centimetres: "168 cm x 272 cm" }
                      ],
                      cargo_tonnage: "25,000 kgs",
                      maximum_permitted_height: "64''",
                      image: "Air_Freight_description03.png"
                  },
                     {
                         modelno: 'A330-300',
                         standard: "5 PMC & 18 AKE",
                         forward_hold: "4 PMC & 6 AKE",
                         aft_hold: "1 PMC & 12 AKE",
                         cargo_volume: "Cargo Volume 14 cubic metres",
                         bulk_load: "",
                         uld_combination: "",
                         doorsizes: [
                            { description: "Forward hold", inches: "66''x106''", centimetres: "169cm x 270 cm" },
                            { description: "Aft hold", inches: "66''x107''", centimetres: "166 cm x 272 cm" },
                            { description: "Bulk", inches: "66''x106''", centimetres: "24 cm x 37 cm" }

                         ],
                         cargo_tonnage: "18,000 kgs",
                         maximum_permitted_height: "64''",
                         image: "Air_Freight_description05.png"
                     },
                          {
                              modelno: 'A330-200',
                              standard: "4 PMC & 14 AKE",
                              forward_hold: "4 PMC & 2 AKE or 14 AKE",
                              aft_hold: "4 PMC OR 12 AKE",
                              cargo_volume: "Cargo Volume 14 cubic metres",
                              bulk_load: "",
                              uld_combination: "",
                              doorsizes: [
                                 { description: "Forward hold", inches: "66''x106''", centimetres: "169cm x 270 cm" },
                                 { description: "Aft hold", inches: "66''x107''", centimetres: "166 cm x 272 cm" },
                                 { description: "Bulk", inches: "24''x106''", centimetres: "63 cm x 96 cm" }

                              ],
                              cargo_tonnage: "16,000 kgs",
                              maximum_permitted_height: "64''",
                              image: "Air_Freight_description06.png"
                          },
                               {
                                   modelno: 'A330-300',
                                   standard: "5 PMC & 18 AKE",
                                   forward_hold: "4 PMC & 6 AKE",
                                   aft_hold: "1 PMC & 12 AKE",
                                   cargo_volume: "Cargo Volume 14 cubic metres",
                                   bulk_load: "",
                                   uld_combination: "",
                                   doorsizes: [
                                      { description: "Forward hold", inches: "66''x106''", centimetres: "169cm x 270 cm" },
                                      { description: "Aft hold", inches: "66''x107''", centimetres: "166 cm x 272 cm" },
                                      { description: "Bulk", inches: "66''x106''", centimetres: "24 cm x 37 cm" }

                                   ],
                                   cargo_tonnage: "18,000 kgs",
                                   maximum_permitted_height: "64''",
                                   image: "Air_Freight_description05.png"
                               },
                                 {
                                     modelno: 'B787',
                                     standard: "06 PMC 12 AKE",
                                     forward_hold: "16 AKE or 05 PMC",
                                     aft_hold: "12 AKE or 04 PAJ",
                                     cargo_volume: "11.4 m2",
                                     bulk_load: "",
                                     uld_combination: "Yes on both hold",
                                     doorsizes: [
                                        { description: "Forward hold", inches: "66''x106''", centimetres: "170cm x 269 cm" },
                                        { description: "Aft hold", inches: "66''x106''", centimetres: "170 cm x 269 cm" },
                                        { description: "Bulk", inches: "35''x44''", centimetres: "170 cm x 269 cm" }

                                     ],
                                     cargo_tonnage: "8600 KG",
                                     maximum_permitted_height: "64 Inches",
                                     image: "Air_Freight_description14.png"
                                 }
        ]
    };

});