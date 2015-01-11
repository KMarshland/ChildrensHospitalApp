
(function(){
    var data = [
        /*Fill in from log library function*/
    ];

    
    function generateAll() {
        var query = "";
        for (var i = 0; i < data.length; i++) {
            query += generateLine(data[i]);
        }
        return query;
    }
    
    function generateLine(marker) {
        return "UPDATE `hospital_project`.`marker_list` " +
            "SET `X` = '" + marker.x + "', `Y` = '" + marker.y + "' WHERE `marker_list`.`ID` = " + marker.id + ";\n";
    }
    
    return generateAll();
})();