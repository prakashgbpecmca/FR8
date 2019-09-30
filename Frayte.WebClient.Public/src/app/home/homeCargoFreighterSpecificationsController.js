
angular.module('ngApp.home').controller('homeCargoFreighterSpecificationsController', function ($scope, $filter) {
    $scope.ComingSoon = 'Coming soon...';

    $scope.toggleCargoFreighter = function () {
        if ($scope.search && $scope.cargoFreighter) {
            var found = $filter('filter')($scope.cargoFreighter, { aircraft: $scope.search });
            if (found.length) {
                return false;
            }
            else {
                return true;
            }
        }
        return false;
    };

    $scope.cargoFreighter = [
       { img: 'A300B4-200F.png', aircraft: 'A300B4-200F', paylond: '44', volume: '280', cargoCompartment: '', mainDoor: '358x259', noseDoor: '', tailDoor: '', bulkDimension: '95x95', note: 'image-A300B4-200F.png' },
       { img: 'A300-600F.png', aircraft: 'A300-600F', paylond: '54', volume: '310', cargoCompartment: '', mainDoor: '358x259', noseDoor: '', tailDoor: '', bulkDimension: '95x95', note: 'image-A300-600F.png' },
       { img: 'A300-600ST.png', aircraft: 'A300-600ST', paylond: '47', volume: '1,400', cargoCompartment: '3,770X710X710', mainDoor: '358x259', noseDoor: '', tailDoor: '', bulkDimension: '95x95', note: 'image-A300-600ST.png' },
       { img: 'A310-200F.png', aircraft: 'A310-200F', paylond: '36', volume: '250', cargoCompartment: '', mainDoor: '358x256', noseDoor: '', tailDoor: '', bulkDimension: '95x108', note: 'image-A310-200F.png' },

       { img: 'A310-300F.png', aircraft: 'A310-300F', paylond: '40', volume: '250', cargoCompartment: '', mainDoor: '358x256', noseDoor: '', tailDoor: '', bulkDimension: '95x108', note: 'image-A310-300F.png' },
       { img: 'A330-200F.png', aircraft: 'A330-200F', paylond: '70', volume: '460', cargoCompartment: '', mainDoor: '369x258', noseDoor: '', tailDoor: '', bulkDimension: '95x107', note: 'image-A330-200F.png' },
       { img: 'A380-800F.png', aircraft: 'A380-800F', paylond: '150', volume: '1,134', cargoCompartment: '', mainDoor: '', noseDoor: '', tailDoor: '', bulkDimension: '', note: 'image-A380-800F.png' },
       { img: 'AN12.png', aircraft: 'AN12', paylond: '18-20', volume: '90', cargoCompartment: '1,355x280x240', mainDoor: '', noseDoor: '', tailDoor: '298x295', bulkDimension: '', note: 'image-AN12.png' },

       { img: 'AN22.png', aircraft: 'AN22', paylond: '80', volume: '700', cargoCompartment: '2,640X382X440', mainDoor: '', noseDoor: '', tailDoor: '300x390', bulkDimension: '', note: 'image-AN22.png' },
       { img: 'AN124.png', aircraft: 'AN124', paylond: '120', volume: '700', cargoCompartment: '3,650x640x440', mainDoor: '640x440', noseDoor: '', tailDoor: '640x440', bulkDimension: '95x107', note: 'image-AN124.png' },
       { img: 'AN225.png', aircraft: 'AN225', paylond: '250', volume: '750', cargoCompartment: '4,335x640x440', mainDoor: '640x440', noseDoor: '', tailDoor: '', bulkDimension: '', note: 'image-AN225.png' },
       { img: 'B707-320C.png', aircraft: 'B707-320C', paylond: '42', volume: '162', cargoCompartment: '1,355x280x240', mainDoor: '310x110', noseDoor: '', tailDoor: '', bulkDimension: '2 @ 122x127 76x89', note: 'image-B707-320C.png' },


       { img: 'B727-100F.png', aircraft: 'B727-100F', paylond: '21', volume: '135', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '137x107 137x112 122x81', note: 'image-B727-100F.png' },
       { img: 'B727-200F.png', aircraft: 'B727-200F', paylond: '26', volume: '182', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '2@ 122x89', note: 'image-B727-200F.png' },
       { img: 'B737-200F.png', aircraft: 'B737-200F', paylond: '17', volume: '77', cargoCompartment: '', mainDoor: '340x219', noseDoor: '', tailDoor: '', bulkDimension: '122x130 122x122', note: 'image-B737-200F.png' },
       { img: 'B737-300F.png', aircraft: 'B737-300F', paylond: '18', volume: '130', cargoCompartment: '', mainDoor: '350x230', noseDoor: '', tailDoor: '', bulkDimension: '122x130 122x122', note: 'image-B737-300F.png' },


       { img: 'B737-400F.png', aircraft: 'B737-400F', paylond: '21', volume: '142', cargoCompartment: '', mainDoor: '360x218', noseDoor: '', tailDoor: '', bulkDimension: '122x130 122x122', note: 'image-B737-400F.png' },
       { img: 'B737x700CF.png', aircraft: 'B737x700CF', paylond: '19', volume: '105', cargoCompartment: '', mainDoor: '340x219', noseDoor: '', tailDoor: '', bulkDimension: '122x89 120x84', note: 'image-B737x700CF.png' },
       { img: 'B737x100F-SF.png', aircraft: 'B737x100F/SF', paylond: '95', volume: '565', cargoCompartment: '', mainDoor: '341x310', noseDoor: '', tailDoor: '', bulkDimension: '112x120', note: 'image-B737x100F-SF.png' },
       { img: 'B737x200F.png', aircraft: 'B737x200F', paylond: '105', volume: '600', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B737x200F.png' },


       { img: 'B747-200BSF-SF.png', aircraft: 'B747-200BSF/SF', paylond: '108', volume: '600', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B747-200BSF-SF.png' },
       { img: 'B737-300SF.png', aircraft: 'B737x300SF', paylond: '106', volume: '615', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B737-300SF.png' },
       { img: 'B737-400F.png', aircraft: 'B737x400F', paylond: '113', volume: '615', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B737-400F2.png' },
       { img: 'B747-400BCF.png', aircraft: 'B747-400BCF', paylond: '110', volume: '615', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B737-8F.png' },


       { img: 'B747-400ERF.png', aircraft: 'B747-400ERF', paylond: '113', volume: '615', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x120', note: 'image-B747LCF.png' },
       { img: 'B737-8F.png', aircraft: 'B737-8F', paylond: '133', volume: '858', cargoCompartment: '', mainDoor: '341x310', noseDoor: '325x244', tailDoor: '', bulkDimension: '112x94', note: 'B747LCF2.png' },
       { img: 'B747LCF.png', aircraft: 'B747LCF', paylond: '80', volume: '1,840', cargoCompartment: 'No Details', mainDoor: '', noseDoor: '', tailDoor: '', bulkDimension: 'No Details', note: 'image-B757-200F.png' },
       { img: 'B757-200F.png', aircraft: 'B757-200F', paylond: '32', volume: '187', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '140x108 140x114', note: 'image-B757-200SF-PCF.png' },


       { img: 'B757-200SF-PCF.png', aircraft: 'B757-200SF/PCF', paylond: '29', volume: '187', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '140x108 140x114', note: 'image-B757-200ERF.png' },
       { img: 'B757-200ERF.png', aircraft: 'B757-200ERF', paylond: '36', volume: '187', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '140x108 140x114', note: 'image-B767-200F.png' },
       { img: 'B767-200F.png', aircraft: 'B767-200F', paylond: '42', volume: '370', cargoCompartment: '', mainDoor: '340x254', noseDoor: '', tailDoor: '', bulkDimension: '97x119', note: 'image-B767-300F.png' },
       { img: 'B767-300F.png', aircraft: 'B767-300F', paylond: '51', volume: '430', cargoCompartment: '', mainDoor: '340x254', noseDoor: '', tailDoor: '', bulkDimension: '97x119', note: 'image-B777-200F.png' },


       { img: 'B777-200F.png', aircraft: 'B777-200F', paylond: '103', volume: '580', cargoCompartment: '', mainDoor: '372x304', noseDoor: '', tailDoor: '', bulkDimension: '90x121', note: 'image-BAe-146-200QTF.png' },
       { img: 'BAe-146-200QTF.png', aircraft: 'BAe 146-200QTF', paylond: '11.5', volume: '78', cargoCompartment: '', mainDoor: '333x193', noseDoor: '', tailDoor: '', bulkDimension: '135x76 91x68', note: 'image-BAe-146-300QTF.png' },
       { img: 'BAe-146-300QTF.png', aircraft: 'BAe 146-300QTF', paylond: '12', volume: '88', cargoCompartment: '', mainDoor: '333x193', noseDoor: '', tailDoor: '', bulkDimension: '135x110 91x105', note: 'image-DC-6F.png' },
       { img: 'DC-6F.png', aircraft: 'DC-6F', paylond: '12.7', volume: '95.7', cargoCompartment: '', mainDoor: '231x170 315x177', noseDoor: '', tailDoor: '', bulkDimension: '2@ 114x94', note: 'image-DC-8-54CF.png' },


       { img: 'DC-8-54CF.png', aircraft: 'DC-8-54CF', paylond: '47', volume: '200', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '4@ 111x91', note: 'image-DC-8-55CF.png' },
       { img: 'DC-8-55CF.png', aircraft: 'DC-8-55CF', paylond: '43', volume: '200', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '4@ 111x91', note: 'image-DC-8-61CF.png' },
       { img: 'DC-8-61CF.png', aircraft: 'DC-8-61CF', paylond: '39', volume: '295', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '2@ 161x138 2@ 111x91', note: 'image-DC-8-63CF.png' },
       { img: 'DC-8-63CF.png', aircraft: 'DC-8-63CF', paylond: '47', volume: '295', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '2@ 161x138 2@ 111x91', note: 'image-DC-8-71CF.png' },


       { img: 'DC-8-71CF.png', aircraft: 'DC-8-71CF', paylond: '42', volume: '295', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '2@ 161x138 2@111x91', note: 'image-DC-8-73CF.png' },
       { img: 'DC-8-73CF.png', aircraft: 'DC-8-73CF', paylond: '49', volume: '295', cargoCompartment: '', mainDoor: '355x215', noseDoor: '', tailDoor: '', bulkDimension: '2@ 161x138 2@111x91', note: 'image-DC-9-15F.png' },
       { img: 'DC-9-15F.png', aircraft: 'DC-9-15F', paylond: '10', volume: '86', cargoCompartment: '', mainDoor: '345x205', noseDoor: '', tailDoor: '', bulkDimension: '134x79 91x76', note: 'image-DC-9-30F.png' },
       { img: 'DC-9-30F.png', aircraft: 'DC-9-30F', paylond: '17', volume: '114', cargoCompartment: '', mainDoor: '345x205', noseDoor: '', tailDoor: '', bulkDimension: '134x79 91x76', note: 'image-DC-10-10F.png' },


       { img: 'DC-10-10F.png', aircraft: 'DC10-10F', paylond: '54', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-DC-10-30F.png' },
       { img: 'DC-10-30F.png', aircraft: 'DC10-30F', paylond: '65', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-DC-10-30CF.png' },
       { img: 'DC-10-30CF.png', aircraft: 'DC10-30CF', paylond: '62', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-DC-10-40F.png' },
       { img: 'DC-10-40F.png', aircraft: 'DC10-40F', paylond: '75', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-Fokker-F27-500.png' },


       { img: 'Fokker-F27-500.png', aircraft: 'Fokker F27-500', paylond: '6.3', volume: '62', cargoCompartment: '1,560x206x190', mainDoor: '232x175 75x165', noseDoor: '', tailDoor: '', bulkDimension: '', note: 'image-Fokker-F27-600.png' },
       { img: 'Fokker-F27-600.png', aircraft: 'Fokker F27-600', paylond: '6.3', volume: '62', cargoCompartment: '1,336x206x190', mainDoor: '288x175 75x165', noseDoor: '', tailDoor: '', bulkDimension: '', note: 'image-Fokker-50.png' },
       { img: 'Fokker-50.png', aircraft: 'Fokker 50', paylond: '7.3', volume: '60', cargoCompartment: '1,596x203x176', mainDoor: '234x177', noseDoor: '', tailDoor: '', bulkDimension: '', note: 'image-IL-18D-Gr-GrM-V.png' },
       { img: 'IL-18D-Gr-GrM-V.png', aircraft: 'IL-18D/Gr/GrM/V', paylond: '13.7', volume: '112', cargoCompartment: '2,400x323x200', mainDoor: '350x185 or 76x140', noseDoor: '', tailDoor: '', bulkDimension: '2@ 125x75', note: 'image-IL-62MF.png' },


       { img: 'IL-62MF.png', aircraft: 'IL-62MF', paylond: '40', volume: '230', cargoCompartment: '', mainDoor: '345x200', noseDoor: '', tailDoor: '107x115', bulkDimension: '131x126 100x126 70x76', note: 'image-IL-76T.png' },
       { img: 'IL-76T.png', aircraft: 'IL-76T', paylond: '42', volume: '288', cargoCompartment: '2,454x345x340', mainDoor: '', noseDoor: '', tailDoor: '345x340', bulkDimension: '', note: 'image-IL-76TD-MD.png' },
       { img: 'IL-76TD-MD.png', aircraft: 'IL-76TD/MD', paylond: '48', volume: '288', cargoCompartment: '2,454x345x340', mainDoor: '', noseDoor: '', tailDoor: '345x340', bulkDimension: '', note: 'image-IL-76TD-90VD.png' },
       { img: 'IL-76TD-90VD.png', aircraft: 'IL-76TD-90VD', paylond: '50', volume: '288', cargoCompartment: '2,454x345x340', mainDoor: '', noseDoor: '', tailDoor: '345x340', bulkDimension: '', note: 'image-IL-96-400T.png' },


       { img: 'IL-96-400T.png', aircraft: 'IL-96-400T', paylond: '92', volume: '580', cargoCompartment: '', mainDoor: '485x287', noseDoor: '', tailDoor: '', bulkDimension: '80x136', note: 'image-IL-100-30.png' },
       { img: 'IL-100-30.png', aircraft: 'IL-100-30', paylond: '17', volume: '100', cargoCompartment: '', mainDoor: '', noseDoor: '', tailDoor: '304x274', bulkDimension: '', note: 'image-IL-188AF-CF.png' },
       { img: 'IL-188AF-CF.png', aircraft: 'IL-188AF/CF', paylond: '14', volume: '106', cargoCompartment: '1,707x304x274', mainDoor: '355x198 243x203', noseDoor: '', tailDoor: '', bulkDimension: '2@ 132x107', note: 'image-MD10-10F.png' },
       { img: 'IL-1011-200F.png', aircraft: 'IL-1011-200F', paylond: '56', volume: '350', cargoCompartment: '', mainDoor: '383x291', noseDoor: '', tailDoor: '', bulkDimension: '111x122', note: 'image-IL-1011-200F.png' },


       { img: 'MD10-10F.png', aircraft: 'MD10-10F', paylond: '65', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-MD10-30F.png' },
       { img: 'MD10-30F.png', aircraft: 'MD10-30F', paylond: '81', volume: '380', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '76x122', note: 'image-MD-11F-CF.png' },
       { img: 'MD-11F-CF.png', aircraft: 'MD-11F/CF', paylond: '90', volume: '500', cargoCompartment: '', mainDoor: '356x259', noseDoor: '', tailDoor: '', bulkDimension: '178x168', note: 'image-MD-80SF.png' },
       { img: 'MD-80SF.png', aircraft: 'MD-80SF', paylond: '', volume: '', cargoCompartment: '', mainDoor: '', noseDoor: '', tailDoor: '', bulkDimension: '', note: '' },


       { img: 'TU-154S.png', aircraft: 'TU-154S', paylond: '18', volume: '72', cargoCompartment: '', mainDoor: '280x187', noseDoor: '', tailDoor: '', bulkDimension: '2@ 135x80', note: 'image-TU-154S.png' },
       { img: 'TU-204C.png', aircraft: 'TU-204C', paylond: '27', volume: '179', cargoCompartment: '', mainDoor: '340x218', noseDoor: '', tailDoor: '', bulkDimension: '2@ 134x120', note: 'image-TU-204C.png' },
       { img: 'Y-8.png', aircraft: 'Y-8', paylond: '20', volume: '90', cargoCompartment: '1,135x350x260', mainDoor: '', noseDoor: '', tailDoor: '298x295', bulkDimension: '', note: 'image-Y-8.png' },
       { img: 'Yak-42D.png', aircraft: 'Yak-42D', paylond: '12', volume: '200', cargoCompartment: '770x215x185', mainDoor: '', noseDoor: '', tailDoor: '', bulkDimension: '2@ 148x135', note: 'image-Yak-42D.png' }
    ];



});