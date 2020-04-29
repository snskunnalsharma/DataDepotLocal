

var objTable;

$(document).ready(function () {



    $("#btn_Book").click(function () {
      
        var AppointmentModel = new Object();
        var Account = $("#accNo").val();
      
        var UrlRecord = $("#GetDetails").val();

        if (Account.length == 0) {
            // var $toast = toastr["error"]("Please enter account number", "Error Notification");
            $("#lbltxtAccount").text("Please enter account number.");
            $("#lbltxtAccount").css('display', 'block');
            return;
        }
        else {

            if (!IsAccount(Account)) {
                $("#lbltxtAccount").text("Please enter only valid account number.");
                $("#lbltxtAccount").css('display', 'block');
                return;
            }                  
            $("#lbltxtAccount").css('display', 'none');
            $.ajax({

                url: UrlRecord,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ account: Account }),
                type: 'POST',
                success: function (data) {

                    //debugger;
                
                    if (data.msg == true) {
                        $("#lbltextError").css('display', 'none');
                        $("#lbltxtAccount").css('display', 'none');
                        window.location.href = data.redirectToUrl;

                    }
                    else {
                       // debugger;
                        $("#lbltextError").text(data.returnMessage);
                        $("#lbltextError").css('display', 'block');
                       // var $toast = toastr["error"](data.returnMessage, "Error Notification");
                    }

                   
                },
                error: function () {
                  
                    //var $toast = toastr["error"](data.returnMessage, "Error Notification"); 
                   // $("#lbltextError").text("Sorry!You will not book appointment.Please contact to Administration");
                   // $("#lbltextError").css('display', 'block');
                   // window.location = url;
                    
                }


            });
        }
    });


});


function IsAccount(Account) {
    var regex = /^[0-9]*$/;                   ///^-?\d*\.?\d+$/;
    return regex.test(Account);
}