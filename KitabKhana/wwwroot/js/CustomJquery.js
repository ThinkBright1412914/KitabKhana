
$('#SearchProduct').keyup(function () {
    var searchValue = $(this).val().toLowerCase();
    $('.col-lg-3').each(function () {
        var productTitle = $(this).find('.card-title#title').text().toLowerCase();
        if (productTitle.indexOf(searchValue) === -1) {
            $(this).fadeOut();
        } else {
            $(this).show();
        }
        //if ($(this).text().search(new RegExp(searchValue, "i")) < 0) {
        //    $(this).fadeOut();
        //}
        //else {
        //    $(this).show()
        //}
    })
})

// !!!!!practical !!!!!! // this function will only select p elemets which falls under card-body
//$('#SearchProduct').keyup(function () {
//    var val = $(this).val().toLowerCase();
//    $('.card-body>p').each(function () {    .card-body:has(p)
//        if ($(this).text().search(new RegExp(val, "i")) < 0) {
//            $(this).fadeOut();
//        }
//        else
//        {
//            $(this).show();
//        }
//    })
//})

$('.searchableDropdown .dropdown-item').click(function () {
    var value = $(this).data('value'); 

    $.ajax({
        url: "/Home/Index",
        method: 'GET', 
        datatype : 'JSON',
        data: { status: value },
        success: function (data) {
            debugger;
          
        },
        error: function (xhr, status, error) {
           
            console.error(error);
        }
    });
});

function GetCategory() {
    $('#Category').click(function () {
        $.ajax({
            url: "/Home/GetCategory",
            datatype: 'JSON',
            method: 'GET',
            success: function (data) {
                debugger;
                var elements = "";
                elements = elements + '<option value = ' + "" + '>' + "" + '</option>';

                $.each(data, function (i, item) {
                    elements = elements + '<option value = ' + item.Id + '>' + item.Name + '</option>'
                })

                $('#Category').empty().append(elements);
            }
        })
    })
}


function showBox() {
    $('.edit').click(function (event) {
        event.preventDefault();
        $('<div id="details-dialog"/>').appendTo('body').dialog({
            close: function (event, ui) {
                $(this).dialog("close");
            },
            title: resources.Details,
            modal: true,
            draggable: true,
            width: 600,
            minHeight: 400,
            maxHeight: 450,
            resizable: true,
            classes: {
                "ui-dialog": "fix-center"
            },
            open: function (event, ui) {
               
            }
        }).load($(this).children(":first")[0].href, {}, function () {

        });
        return false;
    });
}