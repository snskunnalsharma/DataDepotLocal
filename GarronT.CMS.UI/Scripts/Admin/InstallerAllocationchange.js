let map;
var oTable;
var navListItems;
var count = 1;
var CycleID;
var Lat = [];
var Lag = [];
var Address = [];
var AddressID = [];
var Installer, validator;
var Bindproject, BindCycle, BindRoot, BindAddress;

async function initMap() {
    // Request needed libraries.
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(
        "marker",
    );
    var myLatlng = new google.maps.LatLng(39.86911500000000, -100.891880000000) // This is used to center the map to show our markers
    var mapOptions = {
        center: myLatlng,
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        marker: false
    };

    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    map.setOptions({ minZoom: 3, mapId: "DEMO_MAP_ID", });
    var marker = new google.maps.marker.AdvancedMarkerElement({
        position: myLatlng
    });

}

$(document).ready(function () {

    $("#Operations").addClass('active');
    $("#subOperations").addClass('block');
    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    $('#btnExport').prop("disabled", true);
    $('#btnAssignAddress').prop("disabled", true);
    resizeContent();

    AddRowPartial();


    $(window).resize(function () {
        resizeContent();
    });

    $("#SelectedAddress").hide();


    resetDropDown('ddlCycle');
    resetDropDown('ddlRoot');
    resetDropDown('ddlAddress');

    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlVisitStatus").chosen({ no_results_text: "Oops, nothing found!" });


    $('#ddlInstaller').append(
        $('<option></option>').val('0'));




    $("#ddlAssignInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $('#ddlAssignInstaller').append(
        $('<option></option>').val('0'));


    validator = $('#InstallerAllocationForm').validate({

        debug: true, onsubmit: false,
        rules: {
            ProjectName: { required: true },
            Installer: { required: true },
            Dates: { required: true }
        },
        messages: {
            ProjectName: { required: "Select Project" },
            Installer: { required: "Select Installer" },
            Dates: { required: "Select Date" }
        }
    });

    // BindDates()


    //#region Selected menu Active class
    var leftmenuui = $('.sub');
    leftmenuui.find("*").removeClass('active');
    var listate = $('#liInstaller');
    listate.addClass("active");

    //#endregion


    getProjectList();




    //#region address dropdown

    $("#ulAddress").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;
            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }
        },
        selected: function (event, ui) {

            if ($(ui.selected).hasClass('highlighted')) {
                if ($(ui.selected).hasClass('disabledli')) {

                }
                else if ($(ui.selected).hasClass('Appointmentdisabledli')) {

                }

                else {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');
                }


            } else {
                $(ui.selected).addClass('click-selected highlighted');

            }
        },
        unselected: function (event, ui) {

            $(ui.unselected).removeClass('click-selected');
        },
        stop: function (event, ui) {

            $('#btnExport').prop("disabled", true);
            $('#btnShowMap').prop("disabled", true);
            $('#btnSave').prop("disabled", true);

            //selectAddress()

        }
    });

    //#endregion

    //#region Route dropdown

    $("#ulroute").selectable({

        start: function (event, ui) {
            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }
        },
        selected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            }
            
        },
        unselected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                $(ui.unselected).removeClass('click-selected');
            }
            
        },
        stop: function (event, ui) {

            //Address
            //$('#ulAddress').empty();
            //$("#addrHeader").removeClass("ddlHeader").text("Select Street")
            //selectRoute()
        }
    });


    $("#ulRoute1").selectable({

        start: function (event, ui) {
            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }
        },
        selected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            }
        },
        unselected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                $(ui.unselected).removeClass('click-selected');
            }
            
        },
        stop: function (event, ui) {

            //Address
            //$('#ulAddress').empty();
            //$("#addrHeader").removeClass("ddlHeader").text("Select Street")
            //selectRoute()
        }
    });
    //#endregion

    //#region Cycle dropdown

    $("#ulCycle").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }

            var fromDate = $('#datepicker1').val();
            var toDate = $('#datepicker2').val();
            if (fromDate == "") {
                var $toast = toastr["error"]('Please select Start Date first', 'Error Notification');
                return false;
            }
            else if (toDate == "") {
                var $toast = toastr["error"]('Please select End Date first', 'Error Notification');
                return false;

            }
        },
        selected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {
                console.log("in disableli");

            } else {
                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            }
            
        },
        unselected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                $(ui.unselected).removeClass('click-selected');
            }
        },
        stop: function (event, ui) {

            //$('#ulroute').empty();
            //$("#routeHeader").removeClass("ddlHeader").text("Select Route")


            //$('#ulAddress').empty();
            //$("#addrHeader").removeClass("ddlHeader").text("Select Street")

            // selectCycle()

        }
    });

    //#endregion


    $("#ulCycle1").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;

            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }
        },
        selected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            }
        },
        unselected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                $(ui.unselected).removeClass('click-selected');
            }
        },
        stop: function (event, ui) {

            //$('#ulroute').empty();
            //$("#routeHeader").removeClass("ddlHeader").text("Select Route")


            //$('#ulAddress').empty();
            //$("#addrHeader").removeClass("ddlHeader").text("Select Street")

            // selectCycle()

        }
    });

    //#region assign address dropdown

    $("#ulassignAddress").selectable({

        start: function (event, ui) {

            var target = event.target || event.srcElement;
            if (target.tagName == "LI") {
                //$('#items').trigger('unselected')
                //$('#items li').removeClass('click-selected  highlighted');
                //$(ui.unselected).removeClass('click-selected  highlighted');
            }
        },
        selected: function (event, ui) {

            if ($(ui.selected).hasClass('highlighted')) {
                if ($(ui.selected).hasClass('disabledli')) {

                }
                else if ($(ui.selected).hasClass('Appointmentdisabledli')) {

                }
                else {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');
                }


            } else {
                $(ui.selected).addClass('click-selected highlighted');

            }
        },
        unselected: function (event, ui) {
            if ($(ui.selected).hasClass('disabledli')) {

            } else {
                $(ui.unselected).removeClass('click-selected');
            }
        },
        stop: function (event, ui) {

            //var selectedlitext = $('.highlighted').map(function (i, el) {
            //    return $(el)[0].value;
            //});
            //var selectedlilength = selectedlitext.length;
            //if (selectedlilength == 0) {
            //    $("#addrHeader").text("Select Street")
            //} else {
            //    $("#addrHeader").text(selectedlilength + " Address selected")
            //}



            selectassignAddress()



        }
    });
    $("#pIdHideShow").css("display", "none");
    //#endregion
    initMap();

});

function resetDropDown(controlName) {
    $('#' + controlName).multiselect({ includeSelectAllOption: true, enableFiltering: true, enableCaseInsensitiveFiltering: true })
}

function multiSelectReset(controlName) {
    $('#' + controlName).empty();
    $("#" + controlName).multiselect("rebuild");
}


var _ProjectList;

function getProjectList() {

    var GetProject = $('#urlGetProject').val();

    $.ajax({
        type: 'GET',
        url: GetProject,
        async: true,
        // data: { frmDate: fromDate, toDate: toDate },
        success: function (data) {
            _ProjectList = data;
            $('#ddlProject').empty();

            var count = data.length;
            if (count > 0) {

                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlProject').append(
                            $('<option></option>').val('0'));
                    }
                    $('#ddlProject').append(
                        $('<option></option>').val(item.ProjectId).html(item.ProjectCityState)
                    );

                });

            }
            $('.chosen-select').trigger('chosen:updated');

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        },

    });

}
function getCurrentDate() {
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    let current_datetime = new Date()
    let formatted_date = months[current_datetime.getMonth()] + "-" + current_datetime.getDate() + "-" + current_datetime.getFullYear()
    console.log(formatted_date)
    return formatted_date;
}
var prjFromDate;
var prjToDate;

$('#ddlProject').unbind().change(function () {
    debugger;
    //Addedby AniketJ on 23-sep-2016
    oldfromdate = "";
    oldtodate = "";


    //model popup flage set to false i.e. popup will shown
    FromPopupEdit = false;
    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    $('#btnExport').prop("disabled", true);
    $('#btnAssignAddress').prop("disabled", true);
    var Project = $("#ddlProject option:selected")


    if (Project.length > 0) {

        var projectid = Project.val();

        // This condition Applied for TNMP Project
        if (projectid === "10085")
        {
            $("#pIdHideShow").css("display", "block");
        }
        //#region check excel mapped
        var urlCheckLatLongCalculated = $('#urlCheckLatLongCalculatedStatus').val();
        $.ajax({
            type: 'GET',
            url: urlCheckLatLongCalculated,
            data: { projectId: projectid },
            async: true,
            beforeSend: function () {
                //  waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                //  waitingDialog.hide();
            },
            success: function (returndata) {

                if (returndata.success) {
                    var statusnew = returndata.status;
                    var utilitytype = returndata.utilitytype;
                    if (statusnew == 0) {
                        var $toast = toastr["success"]("Latitude & Longitude yet not calculated for this project,you can not proceed further and allocate addresses.", "");
                        $("#btnReset").trigger("click");
                        return false;
                    }
                    else if (statusnew == 2) {
                        var $toast = toastr["success"]("Latitude & Longitude calculation is inprogress,you can not proceed further and allocate addresses.", "");
                        $("#btnReset").trigger("click");
                        return false;
                    }
                    else if (statusnew == 3) {
                        var $toast = toastr["success"]("Latitude & Longitude calculation failed for this project,you can not proceed further and allocate addresses.", "");
                        $("#btnReset").trigger("click");
                        return false;
                    }
                    else if (statusnew == 1) {
                        if (utilitytype == "Electric") {

                        }
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            },

        });


        //#endregion



        $('#ddlInstaller').empty();
        $('.chosen-select').trigger('chosen:updated');

        $("#datepicker1").datepicker("destroy").val("");
        $("#datepicker2").datepicker("destroy").val("");


        //ulCycle
        $('.txtfilterall').val("");
        $('.chbSelectAll').prop('checked', false);

        $('#ulCycle').empty();
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")

        //Route
        $('#ulroute').empty();
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")

        //Address
        $('#ulAddress').empty();
        $("#addrHeader").removeClass("ddlHeader").text("Select Address")





        if ($("#ddlProject option:selected").text() != "None Selected") {
            $("#datepicker1").datepicker("destroy");
            $("#datepicker2").datepicker("destroy");

            var Project = $("#ddlProject option:selected")


            if (Project.length > 0) {

                var projectid = Project.val();
                var currDate = getCurrentDate();
                if (projectid != undefined && projectid != "") {
                    $.each(_ProjectList, function (index, item) {
                        if (item.ProjectId == projectid) {

                            prjFromDate = new Date(item.stringFromDate);
                            prjToDate = new Date(item.stringToDate);
                            //alert(item.stringFromDate + " " + item.stringToDate)

                            $("#datepicker1").datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                onSelect: function () {

                                    var date = $("#datepicker1").val();
                                    if ($.trim(date) != "") {
                                        var newDate1 = new Date(date);
                                        var strfromdate1 = $("#datepicker2").val();
                                        var fromdate = new Date($("#datepicker2").val());

                                        $("#datepicker2").datepicker('option', 'minDate', newDate1);
                                        if (newDate1 > fromdate) {
                                            $("#datepicker2").val(date)
                                        }
                                        else {
                                            $("#datepicker2").val(strfromdate1)
                                        }


                                    }


                                    validator.resetForm();
                                    if (FromPopupEdit == false) {
                                        multiSelectReset('ddlRoot');
                                        multiSelectReset('ddlAddress');

                                        $("#AddressDropDownDiv").slideUp("fast");
                                        $('#MultipleSelectBox_Address').empty();
                                        var $spandiv = $("#btnAddressbtn");
                                        $spandiv.children("span").text("Select Street")
                                        $spandiv.attr("title", "Select Street")



                                        $("#CycleDropDownDiv").slideUp("fast");
                                        $("#MultipleSelectBox_Cycle").children("li").removeClass("selected").removeClass("selecting")
                                        var $spandiv1 = $("#btnCyclebtn");
                                        $spandiv1.children("span").text("Select Cycle")
                                        $spandiv1.attr("title", "Select Cycle")


                                        $("#RouteDropDownDiv").slideUp("fast");
                                        $('#MultipleSelectBox_Route').empty();
                                        var $spandiv2 = $("#btnRoutebtn");
                                        $spandiv2.children("span").text("Select Route")
                                        $spandiv2.attr("title", "Select Route")
                                        //$('#ddlInstaller').trigger("change");
                                        InstallerChange()
                                    }
                                    else {
                                        //set edit mode to true i.e. record is selected to edit from model pop up
                                        editInstallerRecord = true;
                                    }
                                    // initialize()
                                },
                                //dateFormat: 'dd MM yy',
                                dateFormat: 'M-dd-yy',
                            }).datepicker("setDate", prjFromDate);


                            $("#datepicker2").datepicker({
                                changeMonth: true,
                                changeYear: true,
                                minDate: prjFromDate,
                                maxDate: prjToDate,
                                onSelect: function () {
                                    validator.resetForm();

                                    if (FromPopupEdit == false) {
                                        multiSelectReset('ddlRoot');
                                        multiSelectReset('ddlAddress');
                                        $("#AddressDropDownDiv").slideUp("fast");
                                        $('#MultipleSelectBox_Address').empty();
                                        var $spandiv = $("#btnAddressbtn");
                                        $spandiv.children("span").text("Select Street")
                                        $spandiv.attr("title", "Select Street")

                                        $("#CycleDropDownDiv").slideUp("fast");
                                        $("#MultipleSelectBox_Cycle").children("li").removeClass("selected").removeClass("selecting")
                                        var $spandiv1 = $("#btnCyclebtn");
                                        $spandiv1.children("span").text("Select Cycle")
                                        $spandiv1.attr("title", "Select Cycle")

                                        $("#RouteDropDownDiv").slideUp("fast");
                                        $('#MultipleSelectBox_Route').empty();
                                        var $spandiv2 = $("#btnRoutebtn");
                                        $spandiv2.children("span").text("Select Route")
                                        $spandiv2.attr("title", "Select Route")
                                        // $('#ddlInstaller').trigger("change");
                                        InstallerChange()
                                    } else {
                                        //set edit mode to true i.e. record is selected to edit from model pop up
                                        editInstallerRecord = true;
                                    }
                                    // initialize()
                                },
                                dateFormat: 'M-dd-yy',
                            }).datepicker("setDate", prjToDate);
                        }

                    })
                }


                var urlGetInstallerList = $('#urlGetInstallerList').val();


                $.ajax({
                    type: 'GET',
                    url: urlGetInstallerList,
                    data: { projectId: Project.val() },
                    async: true,
                    beforeSend: function () {
                        waitingDialog.show('Loading Please Wait...');
                    },
                    complete: function () {
                        waitingDialog.hide();
                    },
                    success: function (data) {
                        console.log("get installer list success")
                        $('#ddlInstaller').empty();

                        var count = data.length;
                        if (count > 0) {

                            $.each(data, function (index, item) {
                                if (index == 0) {
                                    $('#ddlInstaller').append(
                                        $('<option></option>').val('0'));
                                }
                                $('#ddlInstaller').append(
                                    $('<option></option>').val(item.UserId).html(item.UserName)
                                );
                            });
                            $('.chosen-select').trigger('chosen:updated');


                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        waitingDialog.hide();
                        var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                        console.log('jqXHR:');
                        console.log(jqXHR);
                        console.log('textStatus:');
                        console.log(textStatus);
                        console.log('errorThrown:');
                        console.log(errorThrown);
                    },

                });



                
                var urlCycleData = $('#GetCycle').val();

                var i = 0;
                $.ajax({
                    url: urlCycleData,
                    async: true,
                    data: { ProjectID: Project.val() },
                    success: function (Cycle) {
                        console.log("get urlCycleData success")
                        $('#ulCycle').empty();
                        var count = Cycle.length;
                        for (i = 0; i < count; i++) {

                            $('#ulCycle').append($('<li></li>').val(Cycle[i]).addClass("liCycle active-result").html(Cycle[i]));
                        }
                        //selectCycle()'
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        waitingDialog.hide();
                        var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                        console.log('jqXHR:');
                        console.log(jqXHR);
                        console.log('textStatus:');
                        console.log(textStatus);
                        console.log('errorThrown:');
                        console.log(errorThrown);
                    }
                });
                waitingDialog.hide();
            }

        }
        else {
            $('#ddlInstaller').empty();
            multiSelectReset('ddlCycle');
            multiSelectReset('ddlRoot');
            multiSelectReset('ddlAddress');

        }
        //console.log('call initialize map');
        initialize();

        waitingDialog.hide();

    }
});


function BindDates() {

    var actualDate = new Date();

    var modifiedDate = new Date(actualDate.getFullYear(), actualDate.getMonth() + 1, actualDate.getDate());

    $("#datepicker1").datepicker({
        changeMonth: true,
        changeYear: true,
        //showOn: 'both',
        // minDate: actualDate,
        onSelect: function () {


            var date = $("#datepicker1").val();
            if ($.trim(date) != "") {
                var newDate1 = new Date(date);
                // $("#txtFromDate").val($.datepicker.formatDate('dd-M-yy', newDate));
                $("#datepicker2").datepicker('option', 'minDate', newDate1);
            }

            getProjectList();

            validator.resetForm();

            $('#ddlProject').val(0).attr("selected", "selected");
            $("#ddlProject").multiselect("rebuild");

            $('#ddlInstaller').empty();
            multiSelectReset('ddlCycle');
            multiSelectReset('ddlRoot');
            multiSelectReset('ddlAddress');

            $("#AddressDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Address').empty();
            var $spandiv = $("#btnAddressbtn");
            $spandiv.children("span").text("Select Street")
            $spandiv.attr("title", "Select Street")


            $("#CycleDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Cycle').empty();
            var $spandiv1 = $("#btnCyclebtn");
            $spandiv1.children("span").text("Select Cycle")
            $spandiv1.attr("title", "Select Cycle")

            $("#RouteDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Route').empty();
            var $spandiv2 = $("#btnRoutebtn");
            $spandiv2.children("span").text("Select Route")
            $spandiv2.attr("title", "Select Route")

            //initialize()
        },
        dateFormat: 'M-dd-yy',
    }).datepicker("setDate", actualDate);


    $("#datepicker2").datepicker({
        // buttonImage: '...images/DatePicker_icon.png',
        // buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        //showOn: 'both',
        minDate: actualDate,
        onSelect: function () {

            getProjectList();

            validator.resetForm();
            $('#ddlProject').val(0).attr("selected", "selected");
            $("#ddlProject").multiselect("rebuild");
            //$('#ddlProject').find('option:first').attr('selected', 'selected');
            $('#ddlInstaller').empty();
            multiSelectReset('ddlCycle');
            multiSelectReset('ddlRoot');
            multiSelectReset('ddlAddress');

            $("#AddressDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Address').empty();
            var $spandiv = $("#btnAddressbtn");
            $spandiv.children("span").text("Select Street")
            $spandiv.attr("title", "Select Street")


            $("#CycleDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Cycle').empty();
            var $spandiv1 = $("#btnCyclebtn");
            $spandiv1.children("span").text("Select Cycle")
            $spandiv1.attr("title", "Select Cycle")

            $("#RouteDropDownDiv").slideUp("fast");
            $('#MultipleSelectBox_Route').empty();
            var $spandiv2 = $("#btnRoutebtn");
            $spandiv2.children("span").text("Select Route")
            $spandiv2.attr("title", "Select Route")

            //initialize()
        },
        dateFormat: 'M-dd-yy',
    }).datepicker("setDate", modifiedDate);

}

function resizeContent() {

    var $height = $(document).height() - 210;

    $('#LoadRow').height($height);
    // $('#LoadRow').width($width);
}

function AddRowPartial() {


    var urlgetRowPartial = $('#getRowPartial').val();
    $.get(urlgetRowPartial,
        function (data) {
            $("#LoadRow").append(data)
            //initialize();
        });
}

async function initialize() {

    // Request needed libraries.
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(
        "marker",
    );

    //var lat = document.getElementById('txtlat').value;
    //var lon = document.getElementById('txtlon').value;
    //var myLatlng = new google.maps.LatLng(18.5004866, 73.8668996) // This is used to center the map to show our markers
    var myLatlng = new google.maps.LatLng(39.86911500000000, -100.891880000000) // This is used to center the map to show our markers
    var mapOptions = {
        center: myLatlng,
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        marker: false
    };

    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    map.setOptions({ minZoom: 3, mapId: "DEMO_MAP_ID", });
    var marker = new google.maps.marker.AdvancedMarkerElement({
        position: myLatlng
    });
    //  marker.setMap(map);
    //var oms = new OverlappingMarkerSpiderfier(map,
    //    {
    //        markersWontMove: true,
    //        markersWontHide: true,
    //        basicFormatEvents: true,
    //        keepSpiderfied: true,
    //        circleSpiralSwitchover :20
    //    }
    //); 

    //var markerInfoWin = new google.maps.InfoWindow();
}

var GlobalProjectID;



//var ddlAddressTextList;
//var ddlAddressValueList;
//set edit mode to false i.e. record not selected to edit from model pop up
var editInstallerRecord = false;
var existrecordfound;

//model popup flage set to false i.e. popup will shown
var FromPopupEdit = false; //Addedby AniketJ on 23-Sep-2016
var storeAllInstallerCycle = [];
$('#ddlInstaller').unbind().change(function () {
    debugger;
    waitingDialog.show('Loading Please Wait...');

    storeAllInstallerCycle = [];

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();



    var GetCheckAddressExistby_ProjectInstallerDate = $('#GetCheckAddressExistby_ProjectInstallerDate').val();
    if (fromDate != "" && toDate != "") {
        $.ajax({
            type: "Post",
            url: GetCheckAddressExistby_ProjectInstallerDate,
            async: true,
            data: JSON.stringify({ ProjectID: ProjectID, InstallerID: InstallerID, fromDate: fromDate, toDate: toDate }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (installerdata) {
                console.log("ExcludeSkip == " + installerdata.ExcludeSkip);
                if (installerdata.ExcludeSkip == 1) {
                    console.log("set checkbox status");
                    $("#chkIncludeSkip").prop("checked", false);
                } else {
                    $("#chkIncludeSkip").prop("checked", true);
                }
                if (installerdata.lat_by_ProjectInstaller.length > 0) {
                    
                    existrecordfound = false;
                    $("#InstallerAddrExist").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });
                    console.log("date = " + JSON.stringify(installerdata.lat_by_ProjectInstaller));
                    //#region table

                    oTable = $('#dynamic-table').dataTable({
                        "bInfo": false,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "bFilter": false,
                        "bAutoWidth": false,
                        "bDestroy": true,
                        "bserverSide": true,
                        "bDeferRender": true,
                        "sScrollY": "500px",
                        "sScrollX": "100%",
                        "sScrollXInner": "100%",
                        "sScrollYInner": "100%",

                        "bScrollCollapse": true,
                        //"sAjaxDataProp": 'aData',
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },
                        //"sAjaxSource": msg.result,
                        "aaData": installerdata.lat_by_ProjectInstaller,
                        "aoColumns": [
                            { "sTitle": "Project Name", "mData": "ProjectName", "sWidth": "25%"  },
                            { "sTitle": "Installer Name", "mData": "UserName", "sWidth": "15%"  },
                            { "sTitle": "Cycle", "mData": "Cycle", "sWidth": "5%"  },
                            { "sTitle": "Route", "mData": "Route", "sWidth": "20%"  },
                            { "sTitle": "Start Date", "mData": "strFormDate", "sWidth": "12%"  },
                            { "sTitle": "End Date", "mData": "strToDate", "sWidth": "12%"  },


                            { "bSearchable": false, "mData": null },
                        ],
                        "aoColumnDefs": [
                            {
                                "sTitle": "View/Edit", "sWidth": "5%", 
                                "mRender": function (data, type, full, row) {

                                    storeAllInstallerCycle.push(full.Cycle);

                                    var endDate = new Date(full.strToDate);
                                    var todayDate = new Date(installerdata.todaysDate)
                                    ProjectInstallerlastdate = full.strToDate
                                    if (todayDate > endDate) {
                                        //return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="Edit(this);" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                        return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                    }
                                    else {
                                        //return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="Edit(this);" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                        return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';

                                        // return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.CityID + ' onclick="Activate(' + full.CityID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                    }
                                },
                                "aTargets": [6]
                            }
                        ],

                    }
                    ), $('.dataTables_filter input').attr('maxlength', 50);
                    //#endregion
                    //#endregion
                    waitingDialog.hide();
                    setTimeout(function () {
                        console.log('1')
                        oTable.fnAdjustColumnSizing();
                        console.log('DataTable Testing')
                    }, 500);

                }
                else {
                    $('#ulroute').empty();
                    $("#routeHeader").removeClass("ddlHeader").text("Select Route")


                    $('#ulAddress').empty();
                    $("#addrHeader").removeClass("ddlHeader").text("Select Address")

                    $('#ulCycle').empty();
                    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
                    $('.txtfilterall').val("");
                    $('.chbSelectAll').prop('checked', false);


                    $('#txtfilterRoute').val("");
                    $('#Route_SelectAll').prop('checked', false);
                    $('#Route1_SelectAll').prop('checked', false);

                    $('#txtfilterAddress').val("");
                    $('#Address_SelectAll').prop('checked', false);

                    $('#btnExport').prop("disabled", true);
                    $('#btnShowMap').prop("disabled", true);
                    $('#btnSave').prop("disabled", true);

                    var urlCycleData = $('#GetCycle').val();

                    var i = 0;
                    $.ajax({
                        url: urlCycleData,
                        async: true,
                        data: { "ProjectID": ProjectID },
                        success: function (Cycle) {

                            $('#ulCycle').empty();
                            var count = Cycle.length;
                            for (i = 0; i < count; i++) {

                                $('#ulCycle').append($('<li></li>').val(Cycle[i]).addClass("liCycle active-result").html(Cycle[i]));
                            }
                            waitingDialog.hide();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            waitingDialog.hide();
                            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                            console.log('jqXHR:');
                            console.log(jqXHR);
                            console.log('textStatus:');
                            console.log(textStatus);
                            console.log('errorThrown:');
                            console.log(errorThrown);
                        }
                    });
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }

});

$('#addressfilter').keyup(function () {
    filter(this);
});

function filter(element) {

    var value = $(element).val().toLowerCase();
    $("#MultipleSelectBox_Address > li").each(function () {
        var $this = $(this),
            lower = $this.text().toLowerCase();
        if (lower.indexOf(value) > -1) {
            $this.show();
        } else {
            $this.hide();
        }
    });
}

$(document).click(function (event) {

    if (event.target.parentElement.id != undefined && event.target.parentElement.id != null
        && event.target.parentElement.id != "AddressDropDownDiv"
        && event.target.parentElement.id != "MultipleSelectBox_Address") {
        $("#AddressDropDownDiv").slideUp("fast");
    }

    if (event.target.parentElement.id != undefined && event.target.parentElement.id != null
        && event.target.parentElement.id != "CycleDropDownDiv"
        && event.target.parentElement.id != "MultipleSelectBox_Cycle") {
        $("#CycleDropDownDiv").slideUp("fast");
    }

    if (event.target.parentElement.id != undefined && event.target.parentElement.id != null
        && event.target.parentElement.id != "RouteDropDownDiv"
        && event.target.parentElement.id != "MultipleSelectBox_Route") {
        $("#RouteDropDownDiv").slideUp("fast");
    }

})





var InstallerMap = {};


$("#btnShowMap").unbind("click").click(function () {

    debugger;
    //waitingDialog.show('Loading Please Wait...');

    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select project name first', 'Error Notification');
        return false;
    }
    else if ($('#ddlInstaller').val() < 1) {
        var $toast = toastr["error"]('Please select installer first', 'Error Notification');
        return false;
    }
    else if ($('#ulCycle .highlighted').val() == null || $('#ulCycle .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select cycle first', 'Error Notification');
        return false;
    }
    else if ($('#ulroute .highlighted').val() == null || $('#ulroute .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select route first', 'Error Notification');
        return false;
    }
    else {

        var ddlAddressValueList = [];
        var value = $('#ulAddress li.highlighted')
        value.each(function (index, item) {

            ddlAddressValueList.push($(this).val())

        });

        if ($("#InstallerAllocationForm").valid()) {

            GetLog_Lat_by_Address(ddlAddressValueList);

        }
    }

});

function GetLog_Lat_by_Address(AddressList) {

    var ProjectID = $('#ddlProject option:selected').val();
    var InstallerID = $('#ddlInstaller option:selected').val();

    var GetLatLag = $('#GetLatLag').val();

    $.ajax({
        type: "Post",
        url: GetLatLag,
        async: true,
        data: JSON.stringify({ Address: AddressList, ProjectID: ProjectID, InstallerID: InstallerID }),

        // data: { "Address": Address1 },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            ShowMap(data);

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });


}


var latlongList;
//var bound = new google.maps.LatLngBounds();
async function ShowMap(objectMap) {
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(
        "marker",
    );

    // waitingDialog.show('Loading Please Wait...');
    if (objectMap.length > 0) {

        //$("#btnExport").removeClass("disable_a_href");
        $('#btnExport').prop("disabled", false);
        latlongList = objectMap;

        //var latlngbounds = new google.maps.LatLngBounds();
        var infoWindow = new google.maps.InfoWindow();

        var mapOptions = {
            center: new google.maps.LatLng(objectMap[0].Latitude, objectMap[0].Longitude),
            zoom: 15,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapId: "DEMO_MAP_ID",
        };

        var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        var installername = $('#ddlInstaller option:selected').text();
        var installerid = $('#ddlInstaller option:selected').val();

        var markers = new Array();
        // markers = initMarkers(objectMap);

        var addresscount = 0;
        var icon = {
            url: 'https://maps.google.com/mapfiles/ms/icons/yellow-dot.png', // url
            scaledSize: new google.maps.Size(80, 80), // scaled size
            origin: new google.maps.Point(0, 0), // origin
            anchor: new google.maps.Point(0, 0) // anchor
        };
        // A marker with a with a URL pointing to a PNG.
        

        for (i = 0; i < objectMap.length; i++) {
            addresscount = addresscount + 1;
            var data = objectMap[i];
            var beachFlagImg = document.createElement("img");

            beachFlagImg.src = "https://maps.google.com/mapfiles/ms/icons/yellow-dot.png";
            var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);
            var marker = new google.maps.marker.AdvancedMarkerElement({
                position: myLatlng,
                map: map,
                content: beachFlagImg,
                //content: 'https://chart.googleapis.com/chart?chst=d_map_spin&chld=0.8|0|FFFF42|13||' + addresscount + '',
                //  title: data.Street,
                //animation: google.maps.Animation.DROP,

                //label: {
                //    text: addresscount.toString(),

                //    fontFamily: 'Roboto, Arial, sans-serif, custom-label-' + installerid
                //},

            }
            );
            //marker.setIcon(icon);



            //latlngbounds.extend(myLatlng);
            var name = installername;
            //extend the bounds to include each marker's position
            //bounds.extend(marker.position);

            (function (marker, data) {

                //google.maps.event.addListener(marker, 'click', function (e) {
                google.maps.event.addListener(marker, 'mouseover', function (e) {

                    infoWindow.setContent(data.Street, data.Latitude, data.Longitude);

                    infoWindow.open(map, marker);


                });





                google.maps.event.addListener(marker, 'click', function () {
                    //when the infowindow is open, close it an clear the contents
                    /*var contentStringCal = '';
                    if (contentStringCal == infoWindow.getContent()) {
                        infoWindow.close(map, marker);
                        infoWindow.setContent('');
                    }
                    //otherwise trigger mouseover to open the infowindow
                    else {
                        google.maps.event.trigger(marker, 'mouseover');
                    }*/
                    google.maps.event.trigger(marker, 'mouseover');
                });

                //clear the contents of the infwindow on closeclick
                google.maps.event.addListener(infoWindow, 'closeclick', function () {
                    infoWindow.setContent('');
                });

            })(marker, data);



        }



        /*map.fitBounds(latlngbounds);
        new google.maps.Rectangle({
            bounds: latlngbounds,
            map: map,
            fillColor: "#000000",
            fillOpacity: 0.0,
            strokeWeight: 0
        });*/


    }
    else {

        initialize();
    }

    waitingDialog.hide();
}

$('#btnSave').unbind("click").click(function () {

    debugger;
    if ($('#ddlProject').val() < 1) {
        var $toast = toastr["error"]('Please select project name first', 'Notification');
        return false;
    }
    else if ($('#ddlInstaller').val() < 1) {
        var $toast = toastr["error"]('Please select installer first', 'Notification');
        return false;
    }


    else if ($('#ulCycle .highlighted').val() == null || $('#ulCycle .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select cycle first', 'Notification');
        return false;
    }
    else if ($('#ulroute .highlighted').val() == null || $('#ulroute .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select route first', 'Notification');
        return false;
    }




    waitingDialog.show('Saving data please wait...');

    var ddlAddressValueList = [];
    var ddlAddressValueList1 = $('#ulAddress li.highlighted');
    ddlAddressValueList1.each(function (index, item) { ddlAddressValueList.push($(this).val()) });

    var ddlRootvalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });

    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });

    var ddlRouteList = $("#ddlRoot").val();


    if ($("#InstallerAllocationForm").valid()) {
        var InstallerID = $('#ddlInstaller').val();
        var ProjectID = $('#ddlProject').val();
        var fromDate = $('#datepicker1').val();
        var toDate = $('#datepicker2').val();

        InstallerMap = {
            "RouteList": ddlRouteList,
            "InstallerID": InstallerID,
            "ProjectID": ProjectID,
            "FromDate": fromDate,
            "ToDate": toDate,
            "OldFromDate": oldfromdate,
            "OldToDate": oldtodate,
            "AddressID": ddlAddressValueList,
            "RouteValues": ddlRootvalues,
            "CycleValues": ddlCyclevalues
        }

        var unAlloSkip = "0";
        if ($("#chkUnallocatedSkip").is(":checked"))
        {
            console.log("checked ==> UnAllocated Skip");
            unAlloSkip = "1";
        }
        else
        {
            unAlloSkip = "0";
        }

        var urlSaveInstallerMap = $('#urlSaveInstallerMap').val();

        $.ajax({
            type: 'POST',
            url: urlSaveInstallerMap,
            async: true,
            data: JSON.stringify({ InstallerMap: InstallerMap, unAllocatedSkipStatus: unAlloSkip }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.success == true) {
                    var $toast = toastr["success"](data.returnMessage, "Success Notification");
                }
                else {
                    var $toast = toastr["error"](data.returnMessage, "Error Notification");
                }

                waitingDialog.hide();

                if (data.success == true) {
                    $("#btnShowMap").trigger("click");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }

        });

    }

});


//#region Cancel button on Create Model
$("#btnCancel").click(function () {

    location.href = $('#UrlDashboard').val()

})
//#endregion

//#region Reset button on Create Model
$("#btnReset").click(function () {
    //model popup flage set to false i.e. popup will shown
    FromPopupEdit = false;
    InstallerEdit = false;
    //set edit mode to false i.e. record not selected to edit from model pop up
    editInstallerRecord = false;

    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    $('#btnAssignAddress').prop("disabled", true);
    //$("#btnExport").addClass("disable_a_href");
    $('#btnExport').prop("disabled", true);
    validator.resetForm();
    $("#datepicker1").datepicker("destroy").val("");
    $("#datepicker2").datepicker("destroy").val("");
    //$('#ddlProject').find('option:first').attr('selected', 'selected');
    $('#ddlProject').val(0).attr("selected", "selected");


    $('#ddlInstaller').empty();
    $('#ddlCycle').empty();
    $('#ddlRoot').empty();
    $('#ddlAddress').empty();
    $('.chosen-select').trigger('chosen:updated');

    var actualDate = new Date();
    $("#datepicker1").datepicker("setDate", actualDate);


    //ulCycle

    $('#ulCycle').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $('.txtfilterall').val("");
    $('.chbSelectAll').prop('checked', false);

    //Route
    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")

    //Address
    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address")

    initialize()

    $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');
    $('#btnCyclebtn').parent().removeClass('open');
    $('#ddlroute_chosen').removeClass('chosen-with-drop chosen-container-active');
    $('#btnRoutebtn').parent().removeClass('open');
    $('#ddlassignaddr_chosen').removeClass('chosen-with-drop chosen-container-active');
    $('#btnAddressbtn').parent().removeClass('open');
    $('#ddlCycle1_chosen').removeClass('chosen-with-drop chosen-container-active');



    if ($("#ddladdr_chosen").hasClass('chosen-container-active')) {
        $("#ddladdr_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }
    if ($("#ddlroute_chosen").hasClass('chosen-container-active')) {
        $("#ddlroute_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }
    if ($("#ddlCycle_chosen").hasClass('chosen-container-active')) {
        $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }
    if ($("#ddlCycle1_chosen").hasClass('chosen-container-active')) {
        $("#ddlCycle1_chosen").toggleClass("chosen-with-drop chosen-container-active")
    }

    $("#pIdHideShow").css("display", "none");

})
//#endregion



function CheckAddressExistby_ProjectInstallerDate() {

    existrecordfound = true;

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();



    var GetCheckAddressExistby_ProjectInstallerDate = $('#GetCheckAddressExistby_ProjectInstallerDate').val();
    if (fromDate != "" && toDate != "") {
        $.ajax({
            type: "Post",
            url: GetCheckAddressExistby_ProjectInstallerDate,
            async: true,
            data: JSON.stringify({ ProjectID: ProjectID, InstallerID: InstallerID, fromDate: fromDate, toDate: toDate }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (installerdata) {

                if (installerdata.lat_by_ProjectInstaller.length > 0) {

                    existrecordfound = false;
                    $("#InstallerAddrExist").modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });

                    //#region table

                    oTable = $('#dynamic-table').dataTable(
                        {
                            "bInfo": false,
                            "bPaginate": false,
                            "bLengthChange": false,
                            "bFilter": false,
                            "bAutoWidth": false,
                            "bDestroy": true,
                            "bserverSide": true,
                            "bDeferRender": true,
                            //"sAjaxDataProp": 'aData',
                            "sScrollY": "500px",
                            "bScrollCollapse": true,
                            "oLanguage": {
                                "sLengthMenu": '_MENU_ Records per page'
                            },
                            //"sAjaxSource": msg.result,
                            "aaData": installerdata.lat_by_ProjectInstaller,
                            "aoColumns": [
                                { "mData": "ProjectName" },
                                { "mData": "UserName" },
                                { "mData": "Cycle" },
                                { "mData": "Route" },
                                { "mData": "strFormDate" },
                                { "mData": "strToDate" },

                                { "bSearchable": false, "mData": null },
                            ],
                            "aoColumnDefs": [
                                {
                                    "mRender": function (data, type, full, row) {

                                        var endDate = new Date(full.strToDate);
                                        var todayDate = new Date(installerdata.todaysDate)
                                        ProjectInstallerlastdate = full.strToDate
                                        if (todayDate > endDate) {
                                            return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                        }
                                        else {
                                            return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                            // return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.CityID + ' onclick="Activate(' + full.CityID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                        }
                                    },
                                    "aTargets": [6]
                                }
                            ],

                        }
                    ), $('.dataTables_filter input').attr('maxlength', 50);
                    setTimeout(function () {
                        console.log('2');
                        console.log('DataTable Testing')
                        oTable.fnAdjustColumnSizing();
                    }, 500);
                    //#endregion
                }


            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }


}
var ProjectInstallerlastdate;
$('#btnviewAllInstallers').click(function () {

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = prjFromDate;
    var toDate = prjToDate;

    var GetCheckAddressExistby_ProjectInstallerDate = $('#GetCheckAddressExistby_ProjectInstallerDate').val();

    $.ajax({
        type: "Post",
        url: GetCheckAddressExistby_ProjectInstallerDate,
        async: true,
        data: JSON.stringify({ ProjectID: ProjectID, InstallerID: InstallerID, fromDate: fromDate, toDate: toDate }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (installerdata) {

            if (installerdata.lat_by_ProjectInstaller.length > 0) {


                $("#InstallerAddrExist").modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

                //#region table

                oTable = $('#dynamic-table').dataTable(
                    {
                        "bInfo": false,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "bFilter": false,
                        "bAutoWidth": false,
                        "bDestroy": true,
                        "bserverSide": true,
                        "bDeferRender": true,
                        //"sAjaxDataProp": 'aData',
                        "sScrollY": "500px",
                        "bScrollCollapse": true,
                        "oLanguage": {
                            "sLengthMenu": '_MENU_ Records per page'
                        },
                        //"sAjaxSource": msg.result,
                        "aaData": installerdata.lat_by_ProjectInstaller,
                        "aoColumns": [
                            { "mData": "ProjectName" },
                            { "mData": "UserName" },
                            { "mData": "Cycle" },
                            { "mData": "Route" },
                            { "mData": "strFormDate" },
                            { "mData": "strToDate" },

                            { "bSearchable": false, "mData": null },
                        ],
                        "aoColumnDefs": [
                            {
                                "mRender": function (data, type, full, row) {

                                    var endDate = new Date(full.strToDate);
                                    var todayDate = new Date(installerdata.todaysDate)
                                    ProjectInstallerlastdate = full.strToDate
                                    if (todayDate > endDate) {
                                        return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View" title="View"><i class="fa fa-eye"></i></a> ';
                                    }
                                    else {
                                        return '<a href="#" id="' + full.strFormDate + '" name="' + full.strToDate + '" onclick="EditAllocation(this,\'' + full.Cycle + '\',\'' + full.Route + '\');" class="btn btn-primary btn-xs" role="button" alt="View/Edit" title="View/Edit"><i class="fa fa-eye"></i></a> ';
                                        // return '<a href="#confirmActivatedialog" data-toggle="modal" data-backdrop="static" id=' + full.CityID + ' onclick="Activate(' + full.CityID + ');"  class="btn btn-success btn-xs" role="button" alt="Activate" title="Activate"><i class="fa fa-check"></i></a>';
                                    }
                                },
                                "aTargets": [6]
                            }
                        ],

                    }
                ), $('.dataTables_filter input').attr('maxlength', 50);
                setTimeout(function () {
                    console.log('3')
                    console.log('DataTable Testing')
                    oTable.fnAdjustColumnSizing();
                }, 500);
                //#endregion
            }


        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });
})
$('#btnResetAllInstallers').click(function () {
    //model popup flage set to false i.e. popup will not shown
    debugger;
    FromPopupEdit = true;
    //$("#datepicker1").val("");
    //$("#datepicker2").val("");
    $('#btnAssignAddress').prop("disabled", true);
    //ulCycle

    //$('#ulCycle').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $("li.liCycle").removeClass('ui-selected click-selected highlighted');
    $('#Cycle_SelectAll').prop('checked', false);

    //Route
    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")

    //Address
    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address")



})

$("#btnCloseAllInstallers").click(function (event) {

    $('#ddlProject').trigger("change")

})


//$('li.dropdown.mega-dropdown a').on('click', function (event) {
$('#btnCyclebtn').on('click', function (event) {
    $(this).parent().toggleClass('open');
    $('#btnRoutebtn').parent().removeClass('open');
    $('#btnAddressbtn').parent().removeClass('open');
});

$('#btnRoutebtn').on('click', function (event) {
    $(this).parent().toggleClass('open');
    $('#btnCyclebtn').parent().removeClass('open');
    $('#btnAddressbtn').parent().removeClass('open');
});

$('#btnAddressbtn').on('click', function (event) {
    $(this).parent().toggleClass('open');
    $('#btnCyclebtn').parent().removeClass('open');
    $('#btnRoutebtn').parent().removeClass('open');
});



//#region New code for Address multiselect dropdown
function getAddressList() {

    debugger;
    var projectId = $('#ddlProject').val();

    var ddlRootvalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });

    var ddlAddressValueList = [];
    var ddlAddressValueList1 = $('#ulAddress li.highlighted');
    ddlAddressValueList1.each(function (index, item) { ddlAddressValueList.push($(this).val()) });

    var urlStateData = $('#GetAddressUrl').val();
    var installerid = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();


    $('#ulAddress').empty();

    if (ddlRootvalues != null && ddlRootvalues.length > 0) {

        $.ajax({
            url: urlStateData,
            async: true,
            type: "POST",
            dataType: "json",
            //data: { Root: Root, ProjectID: GlobalProjectID, InstallerID: installerid, selectedDate: selectedD },
            data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: ddlAddressValueList1, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate },
            beforeSend: function () {
                // waitingDialog.hide();
                waitingDialog.show('Loading Street Info. Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (jsonResult) {

                $('#ulAddress').empty();
                $.each(jsonResult.AllAddress, function (index, item) {
                    $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street));
                });

                $.each(jsonResult.InstallerAddress, function (index, item) {

                    if (item.StreetDisabled == true) {

                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                        //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                    }
                    else if (item.Appointmentflag == "1") {
                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                    }
                    else {

                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                    }
                });

                $('#btnExport').prop("disabled", true);
                $('#btnShowMap').prop("disabled", true);
                $('#btnSave').prop("disabled", true);

                selectAddress()
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });




    }
};

function selectAddress() {
    debugger

    waitingDialog.show('Loading Please Wait...');
    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) {
        return $(el)[0].value;
    });

    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
        $('#btnShowMap').prop("disabled", true);
        $('#btnSave').prop("disabled", false);
        $('#btnExport').prop("disabled", true);
    } else {
        $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
        $('#btnShowMap').prop("disabled", false);
        $('#btnSave').prop("disabled", false);
        $('#btnExport').prop("disabled", true);
    }
    debugger
    //#region SelectAll checked/unchecked
    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
        $('#Address_SelectAll').prop('checked', true);
    } else {
        $('#Address_SelectAll').prop('checked', false);
    }
    //#endregion
    //Hide street loader
    waitingDialog.hide();
}

//#region open/close address dropdown
$("#ancaddr_chosen").click(function () {

    if ($("#ddladdr_chosen").hasClass('chosen-container-active')) {
        $("#ddladdr_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddladdr_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

});
//#endregion


function filterAddress(element) {

    var value = $(element).val().toLowerCase();

    $("#ulAddress > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
        $('#Address_SelectAll').prop('checked', true);
    } else {
        $('#Address_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Address_SelectAll').on('click', function () {


    if (this.checked) {
        $("li.liAddress:visible").addClass('ui-selected click-selected highlighted');

        var selectedlitext = $('#ulAddress .highlighted').map(function (i, el) {

            return $(el)[0].value;

        });
        var selectedlilength = selectedlitext.length;
        if (selectedlilength == 0) {
            $("#addrHeader").removeClass("ddlHeader").text("Select Address")
            $('#btnShowMap').prop("disabled", true);
            $('#btnSave').prop("disabled", true);
            $('#btnExport').prop("disabled", true);
        } else {
            $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
            $('#btnShowMap').prop("disabled", false);
            $('#btnSave').prop("disabled", false);
            $('#btnExport').prop("disabled", true);
        }

    }
    else {


        $("li.liAddress:visible").not('.disabledli').not('.Appointmentdisabledli').removeClass('ui-selected click-selected highlighted');

        var selectedlitext = $('#ulAddress .highlighted').map(function (i, el) {

            return $(el)[0].value;

        });
        var selectedlilength = selectedlitext.length;
        if (selectedlilength == 0) {
            $("#addrHeader").removeClass("ddlHeader").text("Select Address")
            $('#btnShowMap').prop("disabled", true);
            $('#btnSave').prop("disabled", false);
            $('#btnExport').prop("disabled", true);
        } else {
            $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")

            $('#btnShowMap').prop("disabled", false);
            $('#btnSave').prop("disabled", false);
            $('#btnExport').prop("disabled", true);
        }
    }
    if (InstallerEdit == true) {
        $('#btnSave').prop("disabled", false);
    }

});

//#endregion

//#region New code for Route multiselect dropdown
function getRouteList() {
    // set map location to current city
    //initialize();

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var selectedDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();

    var cyclevalue = [];
    var value = $('#ulCycle li.highlighted')
    value.each(function (index, item) {
        cyclevalue.push($(this).text())
    });

    

    if (cyclevalue != null && cyclevalue.length > 0) {

        //GetRootByCycle(value[i].text);

        var urlRootData = $('#GetRootByCycle').val();

        $.ajax({
            url: urlRootData,
            type: 'POST',
            async: true,
            data: { ProjectId: ProjectID, "Cycle": cyclevalue, InstallerId: InstallerID, strDateTime: selectedDate, strtoTime: toDate },
            beforeSend: function () {

                waitingDialog.show('Loading Route Info. Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (data) {

                $('#ulroute').empty();

                $.each(data.Route, function (index, item) {
                    $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item));
                });

                if (InstallerID != null) {
                    $.each(data.selectedRoute, function (index, item) {
                        if (item.RouteDisabled == false) {
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else if (item.Appointmentflag != "1") {
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else {
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                        }
                    });
                }
                // waitingDialog.hide();
                selectRoute()

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }
};

function selectRoute() {

    $('#txtfilterAddress').val("");
    $('#Address_SelectAll').prop('checked', false);
    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
    } else {
        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
    }
    //#region SelectAll checked/unchecked
    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
        $('#Route_SelectAll').prop('checked', true);
    } else {
        $('#Route_SelectAll').prop('checked', false);
    }

    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
        $('#Route1_SelectAll').prop('checked', true);
    } else {
        $('#Route1_SelectAll').prop('checked', false);
    }
    //#endregion

    //hide route loader
    waitingDialog.hide();
    getAddressList()
}


//#region open/close Route dropdown
$("#ancRoute1_chosen").click(function () {
    debugger;
    if ($("#ddlRoute1_chosen").hasClass('hosen-with-drop chosen-container-active')) {
        $("#ddlroute1_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlRoute1_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

});

$("#ancroute_chosen").click(function () {
    $("#ddladdr_chosen").removeClass("chosen-with-drop chosen-container-active");
    if ($("#ddlroute_chosen").hasClass('chosen-container-active')) {
        $("#ddlroute_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlroute_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

});

//#endregion

function filterRoute(element) {

    var value = $(element).val().toLowerCase();

    $("#ulroute > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
        $('#Route_SelectAll').prop('checked', true);
    } else {
        $('#Route_SelectAll').prop('checked', false);
    }

    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
        $('#Route1_SelectAll').prop('checked', true);
    } else {
        $('#Route1_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#Route_SelectAll').on('click', function () {
    debugger;

    //Address
    if ($('#ulroute .liRoute').length > 0) {

        $('#ulAddress').empty();
        $("#addrHeader").removeClass("ddlHeader").text("Select Address")

        if (this.checked) {
            $("li.liRoute:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulroute .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }

        }
        else {
            // $("li.liRoute:visible").removeClass('ui-selected click-selected highlighted');
            $("li.liRoute:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');
            var selectedlitext = $('#ulroute .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }
        }

        $('#btnExport').prop("disabled", true);
        $('#btnShowMap').prop("disabled", true);
        $('#btnSave').prop("disabled", true);

        //selectRoute()
    }
    if (InstallerEdit == true) {
        $('#btnSave').prop("disabled", false);
    }
});

$('#Route1_SelectAll').on('click', function () {

    debugger;
    //Address
    if ($('#ulRoute1 .liRoute1').length > 0) {


        if (this.checked) {
            $("li.liRoute1:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulroute1 .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader1").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader1").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }

        }
        else {
            // $("li.liRoute:visible").removeClass('ui-selected click-selected highlighted');
            $("li.liRoute1:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');
            var selectedlitext = $('#ulroute1 .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader1").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader1").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }
        }

    }
});


//#region open/close Cycle dropdown
$("#ancCycle_chosen").click(function () {

    if ($("#ddlCycle_chosen").hasClass('chosen-container-active')) {
        //$("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
        //$("#btnCycleFilter").trigger("click");

        $("#ddlCycle_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }


});


$("#ancCycle1_chosen").click(function () {
    if ($("#ddlCycle1_chosen").hasClass('chosen-with-drop chosen-container-active')) {
        //$("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
        //$("#btnCycleFilter").trigger("click");
        $("#ddlCycle1_chosen").removeClass('chosen-with-drop chosen-container-active');

    }
    else {
        $("#ddlCycle1_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }


});


//#endregion

function filterCycle(element) {

    var value = $(element).val().toLowerCase();

    $("#ulCycle > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#endregion
}



function filterCycle1(element) {
    debugger;
    var value = $(element).val().toLowerCase();

    $("#ulCycle1 > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulCycle1 .liCycle1:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length === $('#ulCycle1 .liCycle1:visible').length) {
        $('#Cycle1_SelectAll').prop('checked', true);
    } else {
        $('#Cycle1_SelectAll').prop('checked', false);
    }
    //#endregion
}



$('#Cycle_SelectAll').on('click', function () {

    if ($('#ulCycle .liCycle').length > 0) {



        var fromDate = $('#datepicker1').val();
        var toDate = $('#datepicker2').val();
        if (fromDate == "") {
            var $toast = toastr["error"]('Please select Start Date first', 'Error Notification');
            return false;
        }
        else if (toDate == "") {
            var $toast = toastr["error"]('Please select End Date first', 'Error Notification');
            return false;
        }

        //Route
        $('#ulroute').empty();
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")

        //Address
        $('#ulAddress').empty();
        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
        if (this.checked) {
            $("li.liCycle:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }
        }
        else {
            $("li.liCycle:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }
        }


        $('#btnExport').prop("disabled", true);
        $('#btnShowMap').prop("disabled", true);
        $('#btnSave').prop("disabled", true);

    }

    if (InstallerEdit == true) {
        $('#btnSave').prop("disabled", false);
    }
});

$('#Cycle1_SelectAll').on('click', function () {

    if ($('#ulCycle1 .liCycle1').length > 0) {
        if (this.checked) {
            $("li.liCycle1:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle1 .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
            } else {
                $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
            }
        }
        else {
            $("li.liCycle1:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle1 .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
            } else {
                $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
            }
        }

    }
});

//#endregion



//Modified by AniketJ on 14-sep-2016
//Export in excel by selecting excel menu
//$('#btnExport').on('click', function () {
$('#lnkExportExcel').on('click', function () {
    //#region --export in excel
    var ProjectID = $('#ddlProject option:selected').val();
    var InstallerID = $('#ddlInstaller option:selected').val();
    var InstallerName = $('#ddlInstaller option:selected').text();
    var FromDate = $('#datepicker1').val();
    var ToDate = $('#datepicker2').val();


    ddlAddressValueList = [];
    var value = $('#ulAddress li.highlighted')
    value.each(function (index, item) {

        ddlAddressValueList.push($(this).val())

    });


    var urlStateData = $('#urlExportInstallerAllocation').val();

    var urlDownloadExcelInstaller = $('#urlDownloadExcelInstaller').val();

    $.ajax({
        url: urlStateData,
        async: true,
        type: "POST",
        data: JSON.stringify({ Address: ddlAddressValueList, ProjectID: ProjectID, InstallerID: InstallerID, InstallerName: InstallerName, FromDate: FromDate, ToDate: ToDate }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        beforeSend: function () {
            waitingDialog.show('Exporting Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (jsonResult) {
            if (jsonResult.status == true) {
                location.href = urlDownloadExcelInstaller;
            }
            else {
                var $toast = toastr["error"](jsonResult.Message, "Error Notification");
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });

    //#endregion
});


//Createdby Aniket on 14-sep-2016
//Export map in Image
$('#lnkExportImage').on('click', function () {
    ExportMapInImage()
});

function convertasbinaryimage1() {


    if (latlongList != undefined && latlongList.length > 0) {
        var center = "center=" + latlongList[0].Latitude + "," + latlongList[0].Longitude + "";
        var latlongarry = [];
        for (i = 0; i < latlongList.length; i++) {
            var markerstring = "&markers=" + latlongList[i].Latitude + "," + latlongList[i].Longitude + "";

            latlongarry.push(markerstring)
        }
        var lastlist = latlongarry.join("");


        var link = 'https://maps.googleapis.com/maps/api/staticmap?' + center + '&zoom=20' + lastlist + '&size=1236x493&sensor=TRUE_OR_FALSE';

        $('#btnExport').attr("href", link)
    }

}

//Createdby Aniket on 14-sep-2016
//#region --Export Map In Image--
function ExportMapInImage() {

    html2canvas(document.getElementById("map_canvas"), {
        logging: false,
        useCORS: true,
        onrendered: function (canvas) {
            canvas.id = "mapcanvas";

            $('#divDownloadMap').empty();
            $('#divDownloadMap').append(canvas)
            $('#mapDownloadDialog').modal({
                show: true,
                backdrop: 'static',
                keyboard: false,
            })
        }
    });

}
//#endregion


//Created by Aniket on 24-Aug-2016

//function to get Installerlist
function GetInstallerForAssignAddress() {
    debugger;
    var urlGetInstallerList = $('#urlGetAssignAddressInstallerList').val();
    var projectID = $("#ddlProject option:selected").val();
    var installerID = $("#ddlInstaller option:selected").val();
    var ProjectId = $('#ddlProject').val();
    var selectedDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();

    multiSelectReset('ddlCycle1');


    $.ajax({
        type: 'GET',
        url: urlGetInstallerList,
        data: { projectId: projectID, InstallerID: installerID },
        async: true,
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            //waitingDialog.hide();
        },
        success: function (data) {
            if (data.success == undefined) {
                $('#ddlAssignInstaller').empty();

                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlAssignInstaller').append($('<option></option>').val('0'));
                    }
                    $('#ddlAssignInstaller').append($('<option></option>').val(item.UserId).html(item.UserName));
                });

                $('.chosen-select').trigger('chosen:updated');
            }
            else {
                var $toast = toastr["error"](data.message, "Notification");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });

    var urlCycleData = $('#GetCycle').val();

    var i = 0;
    $.ajax({
        url: urlCycleData,
        async: true,
        data: { ProjectID: projectID },
        success: function (Cycle) {

            $('#ulCycle1').empty();
            var count = Cycle.length;
            for (i = 0; i < count; i++) {

                $('#ulCycle1').append($('<li></li>').val(Cycle[i]).addClass("liCycle1 active-result").html(Cycle[i]));
                //$("#ulCycle1 li").filter(function () { return $(this).text() === Cycle[i]; }).addClass("ui-selected click-selected highlighted");
            }

            //selectCycle()

        },
        error: function (jqXHR, textStatus, errorThrown) {
            //waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });

    var selectedlivalue = $('#ulCycle1 li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;

    if (selectedlilength == 0) {
        $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
    } else {
        $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
    }
    var cyclevalue = [];
    var value = $('#ulCycle1 li.highlighted');
    value.each(function (index, item) { cyclevalue.push($(this).text()); });
    waitingDialog.hide();
}




$('#ddlVisitStatus').change(function () {

    debugger;
    $('#MultipleSelectBox_Address').empty();
    var startDate = $('#datepicker1').val();
    var endDate = $('#datepicker2').val();
    var urlGetAddressListToAssignInstaller = $('#urlGetAddressListToAssignInstaller').val();
    var projectID = $("#ddlProject option:selected").val();
    var installerID = $("#ddlInstaller option:selected").val();
    var recordStatus = $("#ddlVisitStatus").val();

    $.ajax({
        url: urlGetAddressListToAssignInstaller,
        async: true,
        type: "GET",
        dataType: "json",
        data: { projectId: projectID, InstallerID: installerID, Status: recordStatus, fromDate: startDate, toDate: endDate },
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {

            $('#ulassignAddress').empty();
            $.each(data, function (index, item) {
                $('#ulassignAddress').append($('<li></li>').val(item.ID).addClass("liassignAddress active-result").html(item.Street));
            });
            waitingDialog.show('Loading Please Wait...');

        }, error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
    });
});



$("#btnAssignAddress").click(function () {
    //to open popup
    $("#modelAssignAddress").modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });

    $('#txtfilterassignAddress').val("");
    $('#assignAddress_SelectAll').attr("checked", false);

    $("#assignaddrHeader").removeClass("ddlHeader").text("Select Address")

    $('#ddlVisitStatus').val('0').attr("selected", "selected");
    $('#ddlVisitStatus').trigger("chosen:updated");
    //Get InstallerList and address list
    GetInstallerForAssignAddress();
    //GetAddessToAssignInstaller();
})

//#region Assign address

//#region open/close address dropdown
$("#ancassignaddr_chosen").click(function () {
    $("#ddlassignaddr_chosen").toggleClass("chosen-with-drop chosen-container-active");
});


$('#btnAddressFilter1').click(function () {
    $("#ddlassignaddr_chosen").toggleClass("chosen-with-drop chosen-container-active");
});



//#endregion
//Filtering address
function filterassignAddress(element) {

    var value = $(element).val().toLowerCase();

    $("#ulassignAddress > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulassignAddress .liassignAddress:visible').length > 0 && $('#ulassignAddress .highlighted:visible').length == $('#ulassignAddress .liassignAddress:visible').length) {
        $('#assignAddress_SelectAll').prop('checked', true);
    } else {
        $('#assignAddress_SelectAll').prop('checked', false);
    }
    //#endregion
}

$('#assignAddress_SelectAll').on('click', function () {


    if (this.checked) {
        $("li.liassignAddress:visible").addClass('ui-selected click-selected highlighted');

        var selectedlitext = $('#ulassignAddress .highlighted').map(function (i, el) { return $(el)[0].value; });
        var selectedlilength = selectedlitext.length;
        if (selectedlilength == 0) {
            $("#assignaddrHeader").removeClass("ddlHeader").text("Select Address")

        } else {
            $("#assignaddrHeader").addClass("ddlHeader").text(selectedlilength + " Address selected")

        }

    }
    else {


        $("li.liassignAddress:visible").not('.disabledli').not('.Appointmentdisabledli').removeClass('ui-selected click-selected highlighted');

        var selectedlitext = $('#ulassignAddress .highlighted').map(function (i, el) {

            return $(el)[0].value;

        });
        var selectedlilength = selectedlitext.length;
        if (selectedlilength == 0) {
            $("#assignaddrHeader").removeClass("ddlHeader").text("Select Address")

        } else {
            $("#assignaddrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")


        }
    }


});

function selectassignAddress() {
    debugger
    var selectedlivalue = $('#ulassignAddress li.highlighted').map(function (i, el) {
        return $(el)[0].value;
    });

    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#assignaddrHeader").removeClass("ddlHeader").text("Select Address")

    } else {
        $("#assignaddrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")

    }
    debugger
    //#region SelectAll checked/unchecked
    if ($('#ulassignAddress .liassignAddress:visible').length > 0 && $('#ulassignAddress .highlighted:visible').length == $('#ulassignAddress .liassignAddress:visible').length) {
        $('#assignAddress_SelectAll').prop('checked', true);
    } else {
        $('#assignAddress_SelectAll').prop('checked', false);
    }
    //#endregion
}

//#endregion

$("#btnSaveAssignAddress").click(function () {
    debugger;
    var newinstallerID = $("#ddlAssignInstaller option:selected").val();
    var Project = $("#ddlProject option:selected").val();
    var oldInstallerID = $("#ddlInstaller option:selected").val();

    var ddlAddressValueList = [];
    var ddlAddressValueList1 = $('#ulassignAddress li.highlighted');
    ddlAddressValueList1.each(function (index, item) {
        ddlAddressValueList.push($(this).val());
    });

    var ddlCycleValueList = [];
    var ddlCycleValueList1 = $('#ulCycle1 li.highlighted');
    ddlCycleValueList1.each(function (index, item) {
        ddlCycleValueList.push($(this).val());
    });

    var ddlRouteValueList = [];
    var ddlRouteValueList1 = $('#ulRoute1 li.highlighted');
    ddlRouteValueList1.each(function (index, item) {
        ddlRouteValueList.push($(this).val());
    });

    if (ddlAddressValueList.length > 0) {
        if (newinstallerID != "" && newinstallerID != 0) {

            console.log("oldInstallerId === " + oldInstallerID);
            console.log("newInstallId === " + newinstallerID);
            AddressInstallerMap = {
                "oldInstallerID": oldInstallerID,
                "newInstallerID": newinstallerID,
                "ProjectID": Project,
                "AddressList": ddlAddressValueList,
                "CycleList": ddlCycleValueList,
                "RouteList": ddlRouteValueList
            };
            var urlAssignInstallerMap = $('#urlAssignInstallerMap').val();
            $.ajax({
                type: 'POST',
                url: urlAssignInstallerMap,
                async: true,
                data: JSON.stringify(AddressInstallerMap),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.success == true) {
                        var $toast = toastr["success"](data.returnMessage, "Notification");
                        $("#modelAssignAddress").modal("hide");
                        $("#btnReset").trigger("click");
                        // $('#ddlInstaller').trigger("change");
                        //InstallerChange();
                    }
                    else {
                        var $toast = toastr["error"](data.returnMessage, "Notification");
                    }
                    //waitingDialog.hide();
                    //GetInstallerData()
                    waitingDialog.hide();

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    waitingDialog.hide();
                    var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                    console.log('jqXHR:');
                    console.log(jqXHR);
                    console.log('textStatus:');
                    console.log(textStatus);
                    console.log('errorThrown:');
                    console.log(errorThrown);
                }

            });


        }
        else {
            var $toast = toastr["error"]('Please select Installer', 'Notification');
            return false;
        }
    }
    else {
        var $toast = toastr["error"]('Please select Street', 'Notification');
        return false;
    }
})




var InstallerEdit = false;
var oldfromdate = "";
var oldtodate = "";
function Edit(data) {
    debugger;
    InstallerEdit = true;
    waitingDialog.show('Loading Please Wait...');

    //set edit mode to false i.e. record not selected to edit from model pop up
    //Addedby AniketJ to edit daterange
    oldfromdate = data.id;
    oldtodate = data.name;

    var fromdate = data.id;
    var todate = data.name;

    $("#InstallerAddrExist").modal('hide');
    //model popup flage set to false i.e. popup will not shown

    $("#datepicker1").val(fromdate);
    $("#datepicker2").datepicker('option', 'minDate', new Date(fromdate));
    $("#datepicker2").val(todate);


    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    $('#btnExport').prop("disabled", true);
    $('#btnAssignAddress').prop("disabled", true);


    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $("li.liCycle").removeClass('ui-selected click-selected highlighted');
    $('.txtfilterall').val("");
    $('.chbSelectAll').prop('checked', false);

    //Route
    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route");

    //Address
    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address");

    $('#btnAssignAddress').prop("disabled", false);

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(
        ":checked")) {
        console.log("checked");
        incSkip = "1";
    } else {
        incSkip = "0";
    } 
    console.log("incSkip == " + incSkip);

    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked")) {
        console.log("checked ==> UnAllocated Skip");
        unAlloSkip = "1";
    }
    else {
        unAlloSkip = "0";
    }
    console.log("UnAllocated Skip == " + unAlloSkip);

    var GetExistingCycleList = $('#GetExistingCycleList').val();

    $.ajax({
        type: "Post",
        url: GetExistingCycleList,
        //async: true,
        async: true,
        data: JSON.stringify({ ProjectID: ProjectID, InstallerID: InstallerID, fromDate: fromDate, toDate: toDate }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            debugger;
            //waitingDialog.hide();

            $('#ulCycle').empty();
            var count = data.OrignalCycleList.length;
            for (i = 0; i < count; i++) {
                $('#ulCycle').append($('<li></li>').val(data.OrignalCycleList[i].CycleId).addClass("liCycle active-result").html(data.OrignalCycleList[i].CycleId));
            }


            //select the item whose street are visited.
            //If the street of cycle has record skip or rtu then they are disabled,
            //they can enabled only when that records are assigned to other installer.
            $.each(data.CycleList, function (index, item) {
                debugger;
                if (item.CycleDisabled == false) {
                    $("#ulCycle li").filter(function () { return $(this).text() === item.CycleId; }).addClass("ui-selected click-selected highlighted");
                }
                else if (incSkip == "1")  {
                    $("#ulCycle li").filter(function () { return $(this).text() === item.CycleId; }).addClass("ui-selected click-selected highlighted disabledli");
                }

            });


            $('#txtfilterRoute').val("");
            $('#Route_SelectAll').prop('checked', false);
            $('#Route1_SelectAll').prop('checked', false);

            $('#txtfilterAddress').val("");
            $('#Address_SelectAll').prop('checked', false);


            var selectedlivalue = $('#ulCycle li.highlighted').map(function (i, el) { return $(el).val(); });


            var selectedlilength = selectedlivalue.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }

            //#region SelectAll checked/unchecked
            if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
                $('#Cycle_SelectAll').prop('checked', true);
            } else {
                $('#Cycle_SelectAll').prop('checked', false);
            }
            //#endregion



            // getRouteList()
            //initialize();



            var cyclevalue = [];
            var value = $('#ulCycle li.highlighted')
            value.each(function (index, item) { cyclevalue.push($(this).text()) });

            if (cyclevalue != null && cyclevalue.length > 0) {

                var urlRootData = $('#GetRootByCycle').val();

                $.ajax({
                    url: urlRootData,
                    type: 'POST',
                    async: true,
                    data: { ProjectId: ProjectID, "Cycle": cyclevalue, InstallerId: InstallerID, strDateTime: fromDate, strtoTime: toDate },
                    success: function (data) {


                        $('#ulroute').empty();

                        $.each(data.Route, function (index, item) { $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item)); });

                        if (InstallerID != null) {
                            $.each(data.selectedRoute, function (index, item) {
                                if (item.RouteDisabled == false) {
                                    $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                                }
                                else if (incSkip == "1") {
                                    $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                                }
                            });
                        }

                        //selectRoute()

                        $('#txtfilterAddress').val("");
                        $('#Address_SelectAll').prop('checked', false);
                        var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });


                        var selectedlilength = selectedlivalue.length;
                        if (selectedlilength == 0) {
                            $("#routeHeader").removeClass("ddlHeader").text("Select Route")
                        } else {
                            $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
                        }
                        //#region SelectAll checked/unchecked
                        if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
                            $('#Route_SelectAll').prop('checked', true);
                        } else {
                            $('#Route_SelectAll').prop('checked', false);
                        }

                        if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
                            $('#Route1_SelectAll').prop('checked', true);
                        } else {
                            $('#Route1_SelectAll').prop('checked', false);
                        }
                        //#endregion


                        // getAddressList()

                        var projectId = $('#ddlProject').val();

                        var ddlRootvalues = [];
                        var value = $('#ulroute li.highlighted')
                        value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


                        var ddlCyclevalues = [];
                        var cyclevalue = $('#ulCycle li.highlighted')
                        cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


                        var urlStateData = $('#GetAddressUrl').val();
                        var installerid = $('#ddlInstaller').val();
                        var fromDate = $('#datepicker1').val();
                        var toDate = $('#datepicker2').val();


                        $('#ulAddress').empty();

                        if (ddlRootvalues != null && ddlRootvalues.length > 0) {

                            $.ajax({
                                url: urlStateData,
                                async: true,
                                type: "POST",
                                dataType: "json",
                                data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
                                success: function (jsonResult) {
                                    if (jsonResult.success === undefined) {

                                        // here
                                        $('#ulAddress').empty();
                                        $.each(jsonResult.AllAddress, function (index, item) { $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street)); });

                                        $.each(jsonResult.InstallerAddress, function (index, item) {

                                            if (item.StreetDisabled == true) {

                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                                                //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                                            }
                                            else if (item.Appointmentflag == "1") {
                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                                            }
                                            else {

                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                                            }
                                        });


                                        //selectAddress()

                                        var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                                        var selectedlilength = selectedlivalue.length;
                                        if (selectedlilength == 0) {
                                            $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                                            $('#btnShowMap').prop("disabled", true);
                                            $('#btnSave').prop("disabled", false);
                                            $('#btnExport').prop("disabled", true);
                                        } else {
                                            $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                                            $('#btnShowMap').prop("disabled", false);
                                            $('#btnSave').prop("disabled", false);
                                            $('#btnExport').prop("disabled", true);
                                        }
                                        debugger
                                        //#region SelectAll checked/unchecked
                                        if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                                            $('#Address_SelectAll').prop('checked', true);
                                        } else {
                                            $('#Address_SelectAll').prop('checked', false);
                                        }


                                        waitingDialog.hide();
                                    }
                                    else {
                                        waitingDialog.hide();
                                        var $toast = toastr["error"](jsonResult.message, "Notification");
                                    }

                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    waitingDialog.hide();
                                    var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                                    console.log('jqXHR:');
                                    console.log(jqXHR);
                                    console.log('textStatus:');
                                    console.log(textStatus);
                                    console.log('errorThrown:');
                                    console.log(errorThrown);
                                }
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        waitingDialog.hide();
                        var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                        console.log('jqXHR:');
                        console.log(jqXHR);
                        console.log('textStatus:');
                        console.log(textStatus);
                        console.log('errorThrown:');
                        console.log(errorThrown);
                    }
                });
            }
            waitingDialog.hide();


        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }
        
    });
    //editInstallerRecord = true;



    $('#btnAssignAddress').prop("disabled", false);
    $("#InstallerAddrExist").modal("hide");
}

function EditAllocation(data, cycle, routes) {
    debugger;
    InstallerEdit = true;
    waitingDialog.show('Loading Please Wait...');

    //set edit mode to false i.e. record not selected to edit from model pop up
    //Addedby AniketJ to edit daterange
    oldfromdate = data.id;
    oldtodate = data.name;

    var fromdate = data.id;
    var todate = data.name;

    $("#InstallerAddrExist").modal('hide');
    //model popup flage set to false i.e. popup will not shown

    $("#datepicker1").val(fromdate);
    $("#datepicker2").datepicker('option', 'minDate', new Date(fromdate));
    $("#datepicker2").val(todate);


    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    $('#btnExport').prop("disabled", true);
    $('#btnAssignAddress').prop("disabled", true);


    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    $("li.liCycle").removeClass('ui-selected click-selected highlighted');
    $('.txtfilterall').val("");
    $('.chbSelectAll').prop('checked', false);

    //Route
    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route");

    //Address
    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address");

    $('#btnAssignAddress').prop("disabled", false);

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(
        ":checked")) {
        console.log("checked");
        incSkip = "1";
    } else {
        incSkip = "0";
    }
    console.log("incSkip == " + incSkip);

    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked")) {
        console.log("checked ==> UnAllocated Skip");
        unAlloSkip = "1";
    }
    else {
        unAlloSkip = "0";
    }
    console.log("UnAllocated Skip == " + unAlloSkip);

    if (cycle != null) {
        // do stuff 
        $("#ulCycle li").filter(function () { return $(this).text() === cycle.trim(); }).addClass("ui-selected click-selected highlighted");
        $('#txtfilterRoute').val("");
        $('#Route_SelectAll').prop('checked', false);
        $('#Route1_SelectAll').prop('checked', false);

        $('#txtfilterAddress').val("");
        $('#Address_SelectAll').prop('checked', false);


        var selectedlivalue = $('#ulCycle li.highlighted').map(function (i, el) { return $(el).val(); });


        var selectedlilength = selectedlivalue.length;
        if (selectedlilength == 0) {
            $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
        } else {
            $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
        }

        //#region SelectAll checked/unchecked
        if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
            $('#Cycle_SelectAll').prop('checked', true);
        } else {
            $('#Cycle_SelectAll').prop('checked', false);
        }
        //#endregion
        //initialize();

        var cyclevalue = [];
        var value = $('#ulCycle li.highlighted')
        value.each(function (index, item) { cyclevalue.push($(this).text()) });
        var routeValue = routes.split(',');
        if (cyclevalue != null && cyclevalue.length > 0) {

            var urlRootData = $('#GetAllRouteByCycle').val();

            $.ajax({
                url: urlRootData,
                type: 'POST',
                async: true,
                data: { ProjectId: ProjectID, "Cycle": cyclevalue, "Route": routeValue, "InstallerId": InstallerID },
                success: function (data) {
                    $('#ulroute').empty();
                    debugger;
                    $.each(data.Route, function (index, item) { $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item)); });

                    if (InstallerID != null) {
                        $.each(data.selectedRoute, function (index, item) {
                            if (item.RouteDisabled == false) {
                                $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                            }
                            else if (incSkip == "1") {
                                $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                            }
                        });
                    }

                    //selectRoute()

                    $('#txtfilterAddress').val("");
                    $('#Address_SelectAll').prop('checked', false);
                    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });


                    var selectedlilength = selectedlivalue.length;
                    if (selectedlilength == 0) {
                        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
                    } else {
                        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
                    }
                    //#region SelectAll checked/unchecked
                    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
                        $('#Route_SelectAll').prop('checked', true);
                    } else {
                        $('#Route_SelectAll').prop('checked', false);
                    }

                    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
                        $('#Route1_SelectAll').prop('checked', true);
                    } else {
                        $('#Route1_SelectAll').prop('checked', false);
                    }
                    //#endregion


                    // getAddressList()

                    var projectId = $('#ddlProject').val();

                    var ddlRootvalues = [];
                    var value = $('#ulroute li.highlighted')
                    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


                    var ddlCyclevalues = [];
                    var cyclevalue = $('#ulCycle li.highlighted')
                    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


                    var urlStateData = $('#GetAddressUrl').val();
                    var installerid = $('#ddlInstaller').val();
                    var fromDate = $('#datepicker1').val();
                    var toDate = $('#datepicker2').val();


                    $('#ulAddress').empty();
                    //for stop purpose
                    //waitingDialog.hide();
                    //return;

                    if (ddlRootvalues != null && ddlRootvalues.length > 0) {

                        $.ajax({
                            url: urlStateData,
                            async: true,
                            type: "POST",
                            dataType: "json",
                            data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
                            success: function (jsonResult) {
                                if (jsonResult.success === undefined) {

                                    // here
                                    $('#ulAddress').empty();
                                    $.each(jsonResult.AllAddress, function (index, item) { $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street)); });

                                    $.each(jsonResult.InstallerAddress, function (index, item) {

                                        if (item.StreetDisabled == true) {

                                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                                            //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                                        }
                                        else if (item.Appointmentflag == "1") {
                                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                                        }
                                        else {

                                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                                        }
                                    });


                                    //selectAddress()

                                    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                                    var selectedlilength = selectedlivalue.length;
                                    if (selectedlilength == 0) {
                                        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                                        $('#btnShowMap').prop("disabled", true);
                                        $('#btnSave').prop("disabled", false);
                                        $('#btnExport').prop("disabled", true);
                                    } else {
                                        $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                                        $('#btnShowMap').prop("disabled", false);
                                        $('#btnSave').prop("disabled", false);
                                        $('#btnExport').prop("disabled", true);
                                    }
                                    debugger
                                    //#region SelectAll checked/unchecked
                                    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                                        $('#Address_SelectAll').prop('checked', true);
                                    } else {
                                        $('#Address_SelectAll').prop('checked', false);
                                    }


                                    waitingDialog.hide();
                                }
                                else {
                                    waitingDialog.hide();
                                    var $toast = toastr["error"](jsonResult.message, "Notification");
                                }

                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                waitingDialog.hide();
                                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                                console.log('jqXHR:');
                                console.log(jqXHR);
                                console.log('textStatus:');
                                console.log(textStatus);
                                console.log('errorThrown:');
                                console.log(errorThrown);
                            }
                        });
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    waitingDialog.hide();
                    var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                    console.log('jqXHR:');
                    console.log(jqXHR);
                    console.log('textStatus:');
                    console.log(textStatus);
                    console.log('errorThrown:');
                    console.log(errorThrown);
                }
            });
        }


        //waitingDialog.hide();
        return;
    }

    var GetExistingCycleList = $('#GetExistingCycleList').val();

    $.ajax({
        type: "Post",
        url: GetExistingCycleList,
        async: true,
        data: JSON.stringify({ ProjectID: ProjectID, InstallerID: InstallerID, fromDate: fromDate, toDate: toDate }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            debugger;
            //waitingDialog.hide();

            $('#ulCycle').empty();
            var count = data.OrignalCycleList.length;
            for (i = 0; i < count; i++) {
                $('#ulCycle').append($('<li></li>').val(data.OrignalCycleList[i].CycleId).addClass("liCycle active-result").html(data.OrignalCycleList[i].CycleId));
            }


            //select the item whose street are visited.
            //If the street of cycle has record skip or rtu then they are disabled,
            //they can enabled only when that records are assigned to other installer.
            $.each(data.CycleList, function (index, item) {
                debugger;
                if (item.CycleDisabled == false) {
                    $("#ulCycle li").filter(function () { return $(this).text() === item.CycleId; }).addClass("ui-selected click-selected highlighted");
                }
                else if (incSkip == "1") {
                    $("#ulCycle li").filter(function () { return $(this).text() === item.CycleId; }).addClass("ui-selected click-selected highlighted disabledli");
                }

            });


            $('#txtfilterRoute').val("");
            $('#Route_SelectAll').prop('checked', false);
            $('#Route1_SelectAll').prop('checked', false);

            $('#txtfilterAddress').val("");
            $('#Address_SelectAll').prop('checked', false);


            var selectedlivalue = $('#ulCycle li.highlighted').map(function (i, el) { return $(el).val(); });


            var selectedlilength = selectedlivalue.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }

            //#region SelectAll checked/unchecked
            if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
                $('#Cycle_SelectAll').prop('checked', true);
            } else {
                $('#Cycle_SelectAll').prop('checked', false);
            }
            //#endregion



            // getRouteList()
            //initialize();



            var cyclevalue = [];
            var value = $('#ulCycle li.highlighted')
            value.each(function (index, item) { cyclevalue.push($(this).text()) });

            if (cyclevalue != null && cyclevalue.length > 0) {

                var urlRootData = $('#GetRootByCycle').val();

                $.ajax({
                    url: urlRootData,
                    type: 'POST',
                    async: true,
                    data: { ProjectId: ProjectID, "Cycle": cyclevalue, InstallerId: InstallerID, strDateTime: fromDate, strtoTime: toDate },
                    success: function (data) {


                        $('#ulroute').empty();

                        $.each(data.Route, function (index, item) { $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item)); });

                        if (InstallerID != null) {
                            $.each(data.selectedRoute, function (index, item) {
                                if (item.RouteDisabled == false) {
                                    $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                                }
                                else if (incSkip == "1") {
                                    $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                                }
                            });
                        }

                        //selectRoute()

                        $('#txtfilterAddress').val("");
                        $('#Address_SelectAll').prop('checked', false);
                        var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });


                        var selectedlilength = selectedlivalue.length;
                        if (selectedlilength == 0) {
                            $("#routeHeader").removeClass("ddlHeader").text("Select Route")
                        } else {
                            $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
                        }
                        //#region SelectAll checked/unchecked
                        if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
                            $('#Route_SelectAll').prop('checked', true);
                        } else {
                            $('#Route_SelectAll').prop('checked', false);
                        }

                        if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
                            $('#Route1_SelectAll').prop('checked', true);
                        } else {
                            $('#Route1_SelectAll').prop('checked', false);
                        }
                        //#endregion


                        // getAddressList()

                        var projectId = $('#ddlProject').val();

                        var ddlRootvalues = [];
                        var value = $('#ulroute li.highlighted')
                        value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


                        var ddlCyclevalues = [];
                        var cyclevalue = $('#ulCycle li.highlighted')
                        cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


                        var urlStateData = $('#GetAddressUrl').val();
                        var installerid = $('#ddlInstaller').val();
                        var fromDate = $('#datepicker1').val();
                        var toDate = $('#datepicker2').val();


                        $('#ulAddress').empty();

                        if (ddlRootvalues != null && ddlRootvalues.length > 0) {

                            $.ajax({
                                url: urlStateData,
                                async: true,
                                type: "POST",
                                dataType: "json",
                                data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip },
                                success: function (jsonResult) {
                                    if (jsonResult.success === undefined) {

                                        // here
                                        $('#ulAddress').empty();
                                        $.each(jsonResult.AllAddress, function (index, item) { $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street)); });

                                        $.each(jsonResult.InstallerAddress, function (index, item) {

                                            if (item.StreetDisabled == true) {

                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                                                //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                                            }
                                            else if (item.Appointmentflag == "1") {
                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                                            }
                                            else {

                                                $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                                            }
                                        });


                                        //selectAddress()

                                        var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                                        var selectedlilength = selectedlivalue.length;
                                        if (selectedlilength == 0) {
                                            $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                                            $('#btnShowMap').prop("disabled", true);
                                            $('#btnSave').prop("disabled", false);
                                            $('#btnExport').prop("disabled", true);
                                        } else {
                                            $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                                            $('#btnShowMap').prop("disabled", false);
                                            $('#btnSave').prop("disabled", false);
                                            $('#btnExport').prop("disabled", true);
                                        }
                                        debugger
                                        //#region SelectAll checked/unchecked
                                        if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                                            $('#Address_SelectAll').prop('checked', true);
                                        } else {
                                            $('#Address_SelectAll').prop('checked', false);
                                        }


                                        waitingDialog.hide();
                                    }
                                    else {
                                        waitingDialog.hide();
                                        var $toast = toastr["error"](jsonResult.message, "Notification");
                                    }

                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    waitingDialog.hide();
                                    var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                                    console.log('jqXHR:');
                                    console.log(jqXHR);
                                    console.log('textStatus:');
                                    console.log(textStatus);
                                    console.log('errorThrown:');
                                    console.log(errorThrown);
                                }
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        waitingDialog.hide();
                        var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                        console.log('jqXHR:');
                        console.log(jqXHR);
                        console.log('textStatus:');
                        console.log(textStatus);
                        console.log('errorThrown:');
                        console.log(errorThrown);
                    }
                });
            }
            waitingDialog.hide();


        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }

    });
    //editInstallerRecord = true;



    $('#btnAssignAddress').prop("disabled", false);
    $("#InstallerAddrExist").modal("hide");
}

$('#chkIncludeSkip').change(function () {
    console.log('skip changed');
    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(":checked"))
    {
        $('#chkUnallocatedSkip').prop('checked', false);
        $('#chkUnallocatedSkip').attr('disabled', true);
        console.log("checked");
        incSkip = "1";
    }
    else
    {
        $('#chkUnallocatedSkip').attr('disabled', false);
        incSkip = "0";
    }

    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked"))
    {
        console.log("checked => UnAllocated Skip");
        unAlloSkip = "1";

    }
    else
    {
        unAlloSkip = "0";
    }
    // getAddressList()

    var projectId = $('#ddlProject').val();

    var ddlRootvalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


    var urlStateData = $('#GetAddressUrl').val();
    var installerid = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();


    $('#ulAddress').empty();

    if (ddlRootvalues != null && ddlRootvalues.length > 0) {
        waitingDialog.show('Loading Please Wait...');

        $.ajax({
            url: urlStateData,
            async: true,
            type: "POST",
            dataType: "json",
            data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
            success: function (jsonResult) {
                if (jsonResult.success === undefined) {

                    // here
                    $('#ulAddress').empty();
                    $.each(jsonResult.AllAddress, function (index, item) { $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street)); });

                    $.each(jsonResult.InstallerAddress, function (index, item) {

                        if (item.StreetDisabled == true) {

                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                            //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else if (item.Appointmentflag == "1") {
                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                        }
                        else {

                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                        }
                    });


                    //selectAddress()

                    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                    var selectedlilength = selectedlivalue.length;
                    if (selectedlilength == 0) {
                        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                        $('#btnShowMap').prop("disabled", true);
                        $('#btnSave').prop("disabled", false);
                        $('#btnExport').prop("disabled", true);
                    } else {
                        $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                        $('#btnShowMap').prop("disabled", false);
                        $('#btnSave').prop("disabled", false);
                        $('#btnExport').prop("disabled", true);
                    }
                    debugger
                    //#region SelectAll checked/unchecked
                    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                        $('#Address_SelectAll').prop('checked', true);
                    } else {
                        $('#Address_SelectAll').prop('checked', false);
                    }


                    waitingDialog.hide();
                }
                else {
                    waitingDialog.hide();
                    var $toast = toastr["error"](jsonResult.message, "Notification");
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }
});

$('#chkUnallocatedSkip').change(function () {
    console.log('skip changed');
    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(":checked"))
    {
        console.log("checked");
        incSkip = "1";
    }
    else
    {
        incSkip = "0";
    }

    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked"))
    {
        $('#chkIncludeSkip').attr('checked', false);
        $('#chkIncludeSkip').attr('disabled', true);
        console.log("checked ==> UnAllocated Skip");
        unAlloSkip = "1";
    }
    else
    {
        $('#chkIncludeSkip').attr('disabled', false);
        unAlloSkip = "0";
    }

    // getAddressList()

    var projectId = $('#ddlProject').val();

    var ddlRootvalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });


    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


    var urlStateData = $('#GetAddressUrl').val();
    var installerid = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();


    $('#ulAddress').empty();

    if (ddlRootvalues != null && ddlRootvalues.length > 0) {
        waitingDialog.show('Loading Please Wait...');

        $.ajax({
            url: urlStateData,
            async: true,
            type: "POST",
            dataType: "json",
            data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
            success: function (jsonResult) {
                if (jsonResult.success === undefined) {

                    // here
                    $('#ulAddress').empty();
                    $.each(jsonResult.AllAddress, function (index, item) { $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street)); });

                    $.each(jsonResult.InstallerAddress, function (index, item) {

                        if (item.StreetDisabled == true) {

                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                            //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else if (item.Appointmentflag == "1") {
                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                        }
                        else {

                            $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                        }
                    });


                    //selectAddress()

                    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                    var selectedlilength = selectedlivalue.length;
                    if (selectedlilength == 0) {
                        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                        $('#btnShowMap').prop("disabled", true);
                        $('#btnSave').prop("disabled", false);
                        $('#btnExport').prop("disabled", true);
                    } else {
                        $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                        $('#btnShowMap').prop("disabled", false);
                        $('#btnSave').prop("disabled", false);
                        $('#btnExport').prop("disabled", true);
                    }
                    debugger
                    //#region SelectAll checked/unchecked
                    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                        $('#Address_SelectAll').prop('checked', true);
                    } else {
                        $('#Address_SelectAll').prop('checked', false);
                    }


                    waitingDialog.hide();
                }
                else {
                    waitingDialog.hide();
                    var $toast = toastr["error"](jsonResult.message, "Notification");
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }
});

$("#btnCycleFilter").click(function () {
    debugger;
    waitingDialog.show('Loading Please Wait...');

    //$("#datepicker1").prop("disabled", false);
    //$("#datepicker2").prop("disabled", false);
    //$("#ddlProject").prop('disabled', false).trigger("chosen:updated");
    //$("#ddlInstaller").prop('disabled', false).trigger("chosen:updated");
    //$("#divaddress").removeClass("div-drpdwn-disable");
    //$("#divroute").removeClass("div-drpdwn-disable");

    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")


    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address")


    $('#txtfilterRoute').val("");
    $('#Route_SelectAll').prop('checked', false);
    $('#Route1_SelectAll').prop('checked', false);

    $('#txtfilterAddress').val("");
    $('#Address_SelectAll').prop('checked', false);


    var selectedlivalue = $('#ulCycle li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;

    if (selectedlilength == 0) {
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    } else {
        $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
    }

    //#region SelectAll checked/unchecked
    if ($('#ulCycle .liCycle:visible').length > 0 && $('#ulCycle .highlighted:visible').length == $('#ulCycle .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#endregion

    $('#btnExport').prop("disabled", true);
    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    if (InstallerEdit == true) {
        $('#btnSave').prop("disabled", false);
    }

    // set map location to current city
    //initialize();

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var selectedDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(
        ":checked")) {
        console.log("checked");
        incSkip = "1";
    } else {
        incSkip = "0";
    }
    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked")) {
        console.log("checked => UnAllocated Skip");
        unAlloSkip = "1";
    }
    else {
        unAlloSkip = "0";
    }

    var cyclevalue = [];
    var value = $('#ulCycle li.highlighted');
    value.each(function (index, item) { cyclevalue.push($(this).text()); });

    var ddlAddressValueList = [];
    var ddlAddressValueList1 = $('#ulAddress li.highlighted');
    ddlAddressValueList1.each(function (index, item) { ddlAddressValueList.push($(this).val()); });

    debugger;
    var status = true;
    var check = true;
    for (let elm of storeAllInstallerCycle) {
        if (status) {
            debugger;
            for (let item of cyclevalue) {
                if (elm === item) {
                    status = false;

                    if (confirm('This Cycle is already assigned, if you would like to make changes in route then edit that cycle.')) {
                        $('#dynamic-table tr').each(function (row, tr) {

                            if (item === $(tr).find('td:eq(2)').text()) {
                                selectedDate = $(tr).find('td:eq(4)').text();
                                toDate = $(tr).find('td:eq(5)').text();

                                $("#datepicker1").datepicker("destroy").val("");
                                $("#datepicker2").datepicker("destroy").val("");

                                $("#datepicker1").datepicker("destroy");
                                $("#datepicker2").datepicker("destroy");

                                $("#datepicker1").val(selectedDate);
                                $("#datepicker2").val(toDate);
                                return false;
                            }
                        });
                    }
                    else {
                        debugger;
                        $("#ulCycle").find("li[value='" + item + "']").removeClass("ui-selected click-selected highlighted")
                        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
                        waitingDialog.hide();
                        check = false;
                    }


                    //alert("This Cycle is already assigned, if you would like to make changes in route then edit that cycle.");
                    break;
                }
            }
        }
    }
    debugger;
    if (check) {
        if (cyclevalue != null && cyclevalue.length > 0) {

            var urlRootData = $('#GetRootByCycle').val();

            $.ajax({
                url: urlRootData,
                type: 'POST',
                async: true,
                data: { ProjectId: ProjectID, "Cycle": cyclevalue, InstallerId: InstallerID, strDateTime: selectedDate, strtoTime: toDate },
                success: function (data) {

                    $('#ulroute').empty();

                    $.each(data.Route, function (index, item) { $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item)); });

                    if (InstallerID != null) {
                        $.each(data.selectedRoute, function (index, item) {
                            if (item.RouteDisabled == false) {
                                $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted");
                            }
                            else if (incSkip == "1") {
                                $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                            }
                        });
                    }

                    // selectRoute()

                    $('#txtfilterAddress').val("");
                    $('#Address_SelectAll').prop('checked', false);
                    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });


                    var selectedlilength = selectedlivalue.length;
                    if (selectedlilength == 0) {
                        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
                    } else {
                        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
                    }
                    //#region SelectAll checked/unchecked
                    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
                        $('#Route_SelectAll').prop('checked', true);
                    } else {
                        $('#Route_SelectAll').prop('checked', false);
                    }

                    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
                        $('#Route1_SelectAll').prop('checked', true);
                    } else {
                        $('#Route1_SelectAll').prop('checked', false);
                    }
                    //#endregion


                    //getAddressList()


                    var ddlRootvalues = [];
                    var value = $('#ulroute li.highlighted')
                    value.each(function (index, item) { ddlRootvalues.push($(this).val()); });


                    var urlStateData = $('#GetAddressUrl').val();

                    $('#ulAddress').empty();

                    if (ddlRootvalues != null && ddlRootvalues.length > 0) {

                        $.ajax({
                            url: urlStateData,
                            async: true,
                            type: "POST",
                            dataType: "json",
                            //data: { Root: Root, ProjectID: GlobalProjectID, InstallerID: installerid, selectedDate: selectedD },
                            data: { Root: ddlRootvalues, Cycle: cyclevalue, Address: ddlAddressValueList, ProjectID: ProjectID, installerId: InstallerID, fromDate: selectedDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
                            success: function (jsonResult) {

                                $('#ulAddress').empty();
                                $.each(jsonResult.AllAddress, function (index, item) {
                                    $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street));
                                });

                                $.each(jsonResult.InstallerAddress, function (index, item) {

                                    if (item.StreetDisabled == true) {

                                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                                        //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                                    }
                                    else if (item.Appointmentflag == "1") {
                                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                                    }
                                    else {

                                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                                    }
                                });


                                // selectAddress()

                                var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                                var selectedlilength = selectedlivalue.length;
                                if (selectedlilength == 0) {
                                    $("#addrHeader").removeClass("ddlHeader").text("Select Address");
                                    $('#btnShowMap').prop("disabled", true);
                                    $('#btnSave').prop("disabled", false);
                                    $('#btnExport').prop("disabled", true);
                                } else {
                                    $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected");
                                    $('#btnShowMap').prop("disabled", false);
                                    $('#btnSave').prop("disabled", false);
                                    $('#btnExport').prop("disabled", true);
                                }
                                debugger
                                //#region SelectAll checked/unchecked
                                if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                                    $('#Address_SelectAll').prop('checked', true);
                                } else {
                                    $('#Address_SelectAll').prop('checked', false);
                                }
                                //#endregion

                                waitingDialog.hide();

                                $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');

                                $('#btnCyclebtn').parent().removeClass('open');
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                waitingDialog.hide();
                                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                                console.log('jqXHR:');
                                console.log(jqXHR);
                                console.log('textStatus:');
                                console.log(textStatus);
                                console.log('errorThrown:');
                                console.log(errorThrown);
                            }
                        });

                    }
                    else {
                        waitingDialog.hide();


                        $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');
                        $('#btnCyclebtn').parent().removeClass('open');
                    }


                },
                error: function (jqXHR, textStatus, errorThrown) {
                    waitingDialog.hide();
                    var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                    console.log('jqXHR:');
                    console.log(jqXHR);
                    console.log('textStatus:');
                    console.log(textStatus);
                    console.log('errorThrown:');
                    console.log(errorThrown);
                }
            });

            $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');
            $('#btnCyclebtn').parent().removeClass('open');

        }
        else {
            waitingDialog.hide();
        }
    }
});

$("#btnCycleFilter1").click(function () {
    debugger;

    waitingDialog.show('Loading Please Wait...');

    //$("#datepicker1").prop("disabled", false);
    //$("#datepicker2").prop("disabled", false);
    //$("#ddlProject").prop('disabled', false).trigger("chosen:updated");
    //$("#ddlInstaller").prop('disabled', false).trigger("chosen:updated");
    //$("#divaddress").removeClass("div-drpdwn-disable");
    //$("#divroute").removeClass("div-drpdwn-disable");

    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route");

    var selectedlivalue = $('#ulCycle1 li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;

    if (selectedlilength == 0) {
        $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
    } else {
        $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
    }

    //#region SelectAll checked/unchecked
    if ($('#ulCycle1 .liCycle1:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length == $('#ulCycle1 .liCycle1:visible').length) {
        $('#Cycle1_SelectAll').prop('checked', true);
    } else {
        $('#Cycle1_SelectAll').prop('checked', false);
    }
    //#endregion

    $('#btnExport').prop("disabled", true);
    $('#btnShowMap').prop("disabled", true);
    $('#btnSave').prop("disabled", true);
    if (InstallerEdit == true) {
        $('#btnSave').prop("disabled", false);
    }

    // set map location to current city
    //initialize();

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var selectedDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();


    var cyclevalue = [];
    var value = $('#ulCycle1 li.highlighted')
    value.each(function (index, item) { cyclevalue.push($(this).text()); });

    var ddlAddressValueList = [];
    var ddlAddressValueList1 = $('#ulAddress li.highlighted');
    ddlAddressValueList1.each(function (index, item) { ddlAddressValueList.push($(this).val()); });

    if (cyclevalue != null && cyclevalue.length > 0) {

        var urlRootData = $('#GetRootByCycle').val();

        $.ajax({
            url: urlRootData,
            type: 'POST',
            async: true,
            data: { ProjectId: ProjectID, "Cycle": cyclevalue, InstallerId: InstallerID, strDateTime: selectedDate, strtoTime: toDate },
            success: function (data) {

                $('#ulRoute1').empty();

                $.each(data.Route, function (index, item) { $('#ulRoute1').append($('<li></li>').val(item).addClass("liRoute1 active-result").html(item)); });

                if (InstallerID != null) {
                    $.each(data.selectedRoute, function (index, item) {
                        if (item.RouteDisabled == false) {
                            $("#ulRoute1").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else {
                            $("#ulRoute1").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                        }
                    });
                }

                var selectedlivalue = $('#ulRoute1 li.highlighted').map(function (i, el) { return $(el).val(); });


                var selectedlilength = selectedlivalue.length;
                if (selectedlilength == 0) {
                    $("#RouteHeader1").removeClass("ddlHeader").text("Select Route");
                } else {
                    $("#RouteHeader1").addClass("ddlHeader").text(selectedlilength + " Route selected");
                }
                //#region SelectAll checked/unchecked
                if ($('#ulRoute1 .liRoute1:visible').length > 0 && $('#ulRoute1 .highlighted:visible').length == $('#ulRoute1 .liRoute1:visible').length) {
                    $('#Route1_SelectAll').prop('checked', true);
                } else {
                    $('#Route1_SelectAll').prop('checked', false);
                }
                //#endregion


                //getAddressList()


                var ddlRootvalues = [];
                var value = $('#ulRoute1 li.highlighted');
                value.each(function (index, item) { ddlRootvalues.push($(this).val()) });
                waitingDialog.hide();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
        waitingDialog.hide();


    }
    else {
        $('#ulRoute1').empty();
        $("#RouteHeader1").removeClass("ddlHeader").text("Select Route");
    }



    $('#ddlCycle1_chosen').removeClass('chosen-with-drop chosen-container-active');
    waitingDialog.hide();
    //}

});


$("#btnRouteFilter").click(function () {

    if ($('#ulCycle .highlighted').val() == null || $('#ulCycle .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select cycle first', 'Notification');
        return false;
    }
    waitingDialog.show('Loading Please Wait...');

    $('#ulAddress').empty();
    $("#addrHeader").removeClass("ddlHeader").text("Select Address")


    $('#txtfilterAddress').val("");
    $('#Address_SelectAll').prop('checked', false);
    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });

    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
    } else {
        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
    }
    //#region SelectAll checked/unchecked
    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
        $('#Route_SelectAll').prop('checked', true);
    } else {
        $('#Route_SelectAll').prop('checked', false);
    }
    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
        $('#Route1_SelectAll').prop('checked', true);
    } else {
        $('#Route1_SelectAll').prop('checked', false);
    }
    //#endregion


    //getAddressList()



    var projectId = $('#ddlProject').val();

    var ddlRootvalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRootvalues.push($(this).val()) });
    console.log("root" + ddlRootvalues);

    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).text()) });


    var urlStateData = $('#GetAddressUrl').val();
    var installerid = $('#ddlInstaller').val();
    var fromDate = $('#datepicker1').val();
    var toDate = $('#datepicker2').val();
    var incSkip = "0";
    if ($("#chkIncludeSkip").is(
        ":checked")) {
        console.log("checked");
        incSkip = "1";
    } else {
        incSkip = "0";
    }

    debugger;
    var unAlloSkip = "0";
    if ($("#chkUnallocatedSkip").is(":checked")) {
        console.log("checked => UnAllocated Skip");
        unAlloSkip = "1";
    }
    else {
        unAlloSkip = "0";
    }

    $('#ulAddress').empty();

    if (ddlRootvalues != null && ddlRootvalues.length > 0) {

        $.ajax({
            url: urlStateData,
            async: true,
            type: "POST",
            dataType: "json",
            //data: { Root: Root, ProjectID: GlobalProjectID, InstallerID: installerid, selectedDate: selectedD },
            data: { Root: ddlRootvalues, Cycle: ddlCyclevalues, Address: null, ProjectID: projectId, installerId: installerid, fromDate: fromDate, toDate: toDate, includeSkip: incSkip, unAllocatedSkip: unAlloSkip },
            success: function (jsonResult) {

                $('#ulAddress').empty();
                $.each(jsonResult.AllAddress, function (index, item) {
                    $('#ulAddress').append($('<li></li>').val(item.ID).addClass("liAddress active-result").html(item.Street));
                });

                $.each(jsonResult.InstallerAddress, function (index, item) {

                    if (item.StreetDisabled == true) {

                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted disabledli")
                        //$("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted")
                    }
                    else if (item.Appointmentflag == "1") {
                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted Appointmentdisabledli")
                    }
                    else {

                        $("#ulAddress").find("li[value='" + item.ID + "']").addClass("ui-selected click-selected highlighted ")
                    }
                });


                //selectAddress()


                var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) { return $(el)[0].value; });

                var selectedlilength = selectedlivalue.length;
                if (selectedlilength == 0) {
                    $("#addrHeader").removeClass("ddlHeader").text("Select Address")
                    $('#btnShowMap').prop("disabled", true);
                    $('#btnSave').prop("disabled", false);
                    $('#btnExport').prop("disabled", true);
                } else {
                    $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
                    $('#btnShowMap').prop("disabled", false);
                    $('#btnSave').prop("disabled", false);
                    $('#btnExport').prop("disabled", true);
                }
                debugger
                //#region SelectAll checked/unchecked
                if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
                    $('#Address_SelectAll').prop('checked', true);
                } else {
                    $('#Address_SelectAll').prop('checked', false);
                }
                //#endregion

                waitingDialog.hide();

                $('#ddlroute_chosen').removeClass('chosen-with-drop chosen-container-active');

                $('#btnRoutebtn').parent().removeClass('open');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            }
        });
    }
    else {

        waitingDialog.hide();
        $('#ddlroute_chosen').removeClass('chosen-with-drop chosen-container-active');
        $('#btnRoutebtn').parent().removeClass('open');
    }
})


$("#btnRouteFilter1").click(function () {
    waitingDialog.hide();
    $('#ddlRoute1_chosen').removeClass('chosen-with-drop chosen-container-active');
});

$("#btnAddressFilter").click(function () {

    if ($('#ulCycle .highlighted').val() == null || $('#ulCycle .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select cycle first', 'Notification');
        return false;
    }
    else if ($('#ulroute .highlighted').val() == null || $('#ulroute .highlighted').val().length < 1) {
        var $toast = toastr["error"]('Please select route first', 'Notification');
        return false;
    }


    waitingDialog.show('Loading Please Wait...');
    var selectedlivalue = $('#ulAddress li.highlighted').map(function (i, el) {
        return $(el)[0].value;
    });

    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#addrHeader").removeClass("ddlHeader").text("Select Address")
        $('#btnShowMap').prop("disabled", true);
        $('#btnSave').prop("disabled", false);
        $('#btnExport').prop("disabled", true);
    } else {
        $("#addrHeader").addClass("ddlHeader").text(selectedlilength + " Address Selected")
        $('#btnShowMap').prop("disabled", false);
        $('#btnSave').prop("disabled", false);
        $('#btnExport').prop("disabled", true);
    }
    debugger
    //#region SelectAll checked/unchecked
    if ($('#ulAddress .liAddress:visible').length > 0 && $('#ulAddress .highlighted:visible').length == $('#ulAddress .liAddress:visible').length) {
        $('#Address_SelectAll').prop('checked', true);
    } else {
        $('#Address_SelectAll').prop('checked', false);
    }
    //#endregion
    //Hide street loader
    waitingDialog.hide();
    $('#ddladdr_chosen').removeClass('chosen-with-drop chosen-container-active');
    $('#btnAddressbtn').parent().removeClass('open');

});
