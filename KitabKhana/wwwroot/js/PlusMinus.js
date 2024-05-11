
$('.add').click(function () {
    var id = $(this).data('product-id');
    debugger;
    $.ajax({
        url: '/Cart/plus',
        data: { ids: id },
        success: function (result) {
            // Handle the success result if needed
            console.log(result);
        }
    });
});