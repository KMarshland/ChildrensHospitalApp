var canvas;
var ctx;
var imgWidth;
var imgHeight;
var imageObjs = [];

var floor = 0;
var totalFloors = 7;

var restorePoints = [];

var currentMarker;
var currentX;
var currentY;
//var markerNumber;

var placingMarker;
var placedText;

var allMarkers;
var loadedMarkers = false;

function init() {
    
    floor = (document.URL.split('?')[1] || "floor=0").match(/[f][l][o][o][r][=]\d*/g).last().match(/\d+/g);
    buildFloorSelect();
    buildElevatorMenu();
    
    //graphics stuff
    
    //create the canvas
    document.getElementById("canvas_div").innerHTML += '<canvas id="canvas" width="1" height="1">Canvas is not supported in your browser. </canvas>';
    
    //get the context
    canvas = document.getElementById('canvas');
    ctx = canvas.getContext('2d');
    
    //listen for mouse clicks
    canvas.addEventListener('mousedown', function(evt) {
        var mousePos = getMousePos(evt);
        tryClick(mousePos.x, mousePos.y);
      }, false
    );
    
    
    for (var i = 0; i < totalFloors; i++){
        var imageObj = new Image();
    
        imageObjs.push({
            obj: imageObj,
            id: i,
            loaded: false
        });
        
        imageObj.onload = function() {
            //there is no elegance here, only sleep deprivation and regret
            var it = this.src.match(/\d[\.][p][n][g]/g).last().match(/\d*/g)[0];//HACK HACK HACK
            if (it == floor) {
                setFloor(floor);
            }
            imageObjs[it].loaded = true;
        };
        imageObj.src = 'http://www.marshlandgames.com/HospitalProject/Client/MarkerPlacement/FloorMaps/' +
            'CMHOFloor' + (i) + '.png';
        
    }
    
    loadExistingMarkers();
}

function setFloor(fl) {
    floor = +fl;
    drawFloor();
    window.history.pushState("", "", document.URL.split('?')[0] + '?floor='+floor);
    document.querySelector('#floorSelect').value = "" + floor;
}

function onFloorSelectChange() {
    setFloor(parseInt(document.getElementById('floorSelect').value));
}

function buildFloorSelect() {
    var ninner = ''
    for (var i = 0; i < totalFloors; i++){
        if (i == 0) {
            ninner += '<option value="0">Ground floor</option>';
        } else {
            ninner += '<option value="' + i + '">Floor ' + i + '</option>'
        }
    }
    
    
    document.getElementById('floorSelect').innerHTML = ninner;
}

function buildElevatorMenu() {
    $('#edit_elevator').on('change', function(){
        checkElevatorMenu();
    });
    $('#create_elevator').on('change', function(){
        checkElevatorMenu();
    });
    checkElevatorMenu();
    
    var ninner = ''
    for (var i = 0; i < totalFloors; i++){
        if (i == 0) {
            ninner += '<input type="checkbox" name="elevator_floor" value="0">Ground floor<br />';
        } else {
            ninner += '<input type="checkbox" name="elevator_floor" value="' + i + '">Floor ' + i + '<br />'
        }
    }
    $('#elevatorOptions').html(ninner);
    
    $('#createElevatorOptions').html(ninner.replace(/elevator_floor/, 'create_elevator_floor'));
}

function checkElevatorMenu(){
    if ($('#edit_elevator').is(':checked')) {
        $('.elevatorOptions').slideDown();
    } else {
        $('.elevatorOptions').slideUp();
    }
    
    if ($('#create_elevator').is(':checked')) {
        $('.createElevatorOptions').slideDown();
    } else {
        $('.createElevatorOptions').slideUp();
    }
}

function drawFloor() {
    imgWidth = imageObjs[floor].obj.width;
    imgHeight = imageObjs[floor].obj.height;
    
    canvas.width = imgWidth;
    canvas.height = imgHeight;
    
    ctx.drawImage(imageObjs[floor].obj, 0, 0, imgWidth, imgHeight);
    
    if (loadedMarkers) {
        drawMarkers();
    }
}

function drawPoint(x, y){
    var size = 8;
    ctx.fillStyle = "rgb(0,255,0)";
    ctx.fillRect (x - size/2, y - size/2, size, size);
}

function drawPointColor(x, y, colorstring){
    var size = 8;
    ctx.fillStyle = colorstring;
    ctx.fillRect (x - size/2, y - size/2, size, size);
}

function drawText(x, y, text) {
    ctx.font="20px Georgia bold";
    ctx.lineWidth = 0.5;
    ctx.strokeStyle = "rgb(255,255,255)";
    ctx.fillStyle = "rgb(0,0,0)";
    
    ctx.fillText(text,x,y);
    ctx.strokeText(text,x,y);
    
    ctx.fill();
    ctx.stroke();
}

function drawSpecialPoint(x,y){
    var size = 8;
    ctx.fillStyle = "rgb(255,0,0)";
    ctx.fillRect (x - size/2, y - size/2, size, size);
    
    size /= 2;
    ctx.fillStyle = "rgb(0,255,255)";
    ctx.fillRect (x - size/2, y - size/2, size, size);
}

function getMousePos(evt) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: evt.clientX - rect.left,
        y: evt.clientY - rect.top
    };
}

function tryClick(x, y) {
    if (placingMarker) {
        placeMarker(x,y);
    } else {
        //alert (x + ", " + y + "      ");
        //alert(markerWithin(allMarkers[1], 597, 199));
        for (var i = 0; i < allMarkers.length; i++){
            if (markerWithin(allMarkers[i], x, y)) {
                document.getElementById('which_marker').value = allMarkers[i].id;
                reselectMarker();
                break;
            }
        }
    }
}

function displayText(x,y,text) {
    var el = document.getElementById('popup_div');
    var rect = canvas.getBoundingClientRect();
    
    el.style.top = (y + rect.top).toString() + 'px';
    el.style.left = (x + rect.left).toString() + 'px';
    
    el.innerHTML = text;
}

function placeMarker(x, y) {
    placingMarker = false;
    
    drawPoint(x,y);
    createMarker(x, y);
    
    document.getElementById('new_text').value = '';
    
}

function startPlaceMarker() {
    placingMarker = true;
    placedText = document.getElementById('new_text').value;
}

function loadExistingMarkers(){
    allMarkers = [];
    loadMarkersSpeedy();
}

function loadMarkersSpeedy() {
    
    request("show_formatted_markers.php", function(resp){
        var lines = resp.split("<br />");
        
        for (var i = 0; i < lines.length; i++){
            var formatted = formatMarkerGet(lines[i]);
            allMarkers.push(formatted);
        }
        
        loadedMarkers = true;
        
        drawMarkers();
        buildMarkerSelect();
        
        setFloor(floor);
    });
}

function drawMarkers(){
    for (var i = 0; i < allMarkers.length; i++){
        var formatted = allMarkers[i];
        if (formatted.showing()) {
            if (formatted.priority == 1) {
                drawPoint(formatted.x, formatted.y);
            } else if (formatted.priority == 2) {
                drawPointColor(formatted.x, formatted.y, 'rgb(0,0,255)');
            }
            
            //drawText(formatted.x, formatted.y, (formatted.id) + '.' + formatted.text);
        }
    }
    
    reselectMarker();
}

function buildMarkerSelect() {
    var mainEl = document.getElementById('which_marker');
    
    for (var i = 0; i < allMarkers.length; i++){
        if (allMarkers[i].showing()) {
            mainEl.innerHTML += '' +
                '<option value="' + (allMarkers[i].id) + '">' +
                /*(allMarkers[i].id) + '. ' + */allMarkers[i].text +
                '</option>' +
            '';
        }
    }
    
    reselectMarker();
}

function reselectMarker() {
    
    if (currentFormatted() != null) {
        if (currentFormatted().showing()) {
            drawPointColor(currentX, currentY, currentFormatted().priority == 1 ? 'rgb(0,255,0)' : 'rgb(0,0,255)');
        }
    }
    
    
    var newcurrent = document.getElementById('which_marker').value;
    //getMarker(newcurrent + "");
    setCurrent(newcurrent);
}

function createMarker(x, y) {
    var selectedFloors = [];
    $('#createElevatorOptions input:checked').each(function() {
        selectedFloors.push($(this).attr('value'));
    });
    var selectedFloorsString = selectedFloors.join(escape(','));
    
    var args = {
        floor: 0+floor,
        x: x,
        y: y,
        newText: document.getElementById('new_text').value,
        priority: 1+document.getElementById('create_priority').checked,
        elevator:0+document.getElementById('create_elevator').checked,
        floors: selectedFloorsString
    };
    createMarkerArgs(args);
}

function createMarkerArgs(args) {
    request("create_marker.php?text="+args.newText+
            "&x="+args.x+
            "&y="+args.y+
            "&floor="+args.floor+
            "&priority="+args.priority+
            "&elevator="+args.elevator+
            "&floors="+args.floors,
    function(resp){
        reloadPage();
    });
}

function editMarker() {
    var selectedFloors = [];
    $('#elevatorOptions input:checked').each(function() {
        selectedFloors.push($(this).attr('value'));
    });
    var selectedFloorsString = selectedFloors.join(escape(','));
    
    editMarkerArgs(
            currentMarker,
            document.getElementById('edit_text').value,
            1+document.getElementById('edit_priority').checked,
            0+document.getElementById('edit_elevator').checked,
            selectedFloorsString
    );
}

function editMarkerArgs(id, newText, newPriority, newIsElevator, newFloorsString) {
    var url = "edit_marker.php?" +
            "q=" + id +
            "&text=" + newText +
            "&priority=" + newPriority +
            "&elevator=" + newIsElevator + 
            "&floors=" + newFloorsString + 
    "";
    //alert(url);
    request(url, function(resp){
        reloadPage();
    });
}

function deleteMarker() {
    deleteMarkerArgs(currentMarker,
                   document.getElementById('edit_text').value);
}

function deleteMarkerArgs(id, newText) {
    editMarkerArgs(id, newText, 0);
}

function setCurrent(nc){
    
    if (nc == null || nc == "") {
        return;
    }
    
    currentMarker = nc;
    
    if (currentFormatted() == null) {
        return;
    }
    
    currentX = currentFormatted().x;
    currentY = currentFormatted().y;
    
    document.getElementById('edit_text').value = currentFormatted().text;
    document.getElementById('edit_priority').checked = currentFormatted().priority - 1;
    document.getElementById('edit_elevator').checked = currentFormatted().isElevator;
    
    if (currentFormatted().showing()) {
        drawSpecialPoint(currentX, currentY);
    }
    
    checkElevatorMenu();
    
    var selectedFloors = currentFormatted().onFloors.split(',');
    $('#elevatorOptions').find('input').prop('checked', false);
    for (var i = 0; i < selectedFloors.length; i++){
        $('#elevatorOptions').find('[value="' + selectedFloors[i] + '"]').prop('checked', true);
    }
}

function markerWithin(marker, x, y) {
    
    var sizeDiv2 = 4;
    var within = (marker.x < x + sizeDiv2) && (marker.x > x - sizeDiv2) &&
        (marker.y < y + sizeDiv2) && (marker.y > y - sizeDiv2);
    return within;
}

function request(url, callback) {    
    if (window.XMLHttpRequest){// code for IE7+, Firefox, Chrome, Opera, Safari
      xmlhttp=new XMLHttpRequest();
    } else {// code for IE6, IE5
      xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
    }
    
    xmlhttp.onreadystatechange=function()
      {
      if (xmlhttp.readyState==4 && xmlhttp.status==200)
        {
            callback(xmlhttp.responseText);
        }
      }
    xmlhttp.open("GET",url,true);
    xmlhttp.send();
}

function reloadPage(){
    window.location = document.URL.split('?')[0] + '?floor='+floor;
}

function formatMarkerGet(str) {
    var res = str.split("|");
    
    if (res.length == 1) {
        return {
            text: res[0],
            displaying: false,
            showing: function(){
                return false;
            }
        }
    }
    
    return {
        raw: {str: str, split: res},
        id: res[0],
        text: res[1],
        x: parseInt(res[2]),
        y: parseInt(res[3]),
        priority: parseInt(res[4]),
        displaying: (parseInt(res[4]) != 0),
        floor: parseInt(res[5]),
        isElevator: (parseInt(res[6]) != 0),
        onFloors: res[7],
        parsedOnFloors: (function(){
            var parts = res[7].split(',');
            for (var i = 0; i < parts.length; i++){
                parts[i] = parseInt(parts[i]);
            }
            return parts;
        })(),
        
        showing: function(){
            if (!this.displaying) {//it HAS to be displaying. This is whether or not it's been deleted
                return false;
            }
            
            if (this.isElevator) {
                for (var i = 0; i < this.parsedOnFloors.length; i++){
                    if (this.parsedOnFloors[i] == floor) {
                        return true;
                    }
                }
            } else {
                return this.floor == floor;
            }
            
            return false;
        }
    }
}

function currentFormatted(){
    
    if (typeof (MARKER_DICT) == 'undefined') {
        MARKER_DICT = {};
    }
    
    if (MARKER_DICT[currentMarker + ""] != null) {
        return MARKER_DICT[currentMarker + ""];
    }
    
    for (var i = 0; i < allMarkers.length; i++){
        if (allMarkers[i].id + "" == currentMarker + "") {
            MARKER_DICT[allMarkers[i].id + ""] = allMarkers[i];
            return allMarkers[i];
        }
    }
    return null;
}

if (!Array.prototype.last){
    Array.prototype.last = function(){
        return this[this.length - 1];
    };
};

