var dataTable;
$(document).ready(function () {
    LoadData();
})

function LoadData() {
    var dataTable =
        $('#tblProduct').DataTable({
            "ajax": {
                "url": "/Admin/Company/GetAll",
            },
            "columns":
                [
                    { "data": "name", "width": "15%" },
                    { "data": "address", "width": "15%" },
                    { "data": "city", "width": "15%" },
                    { "data": "state", "width": "15%" },
                    { "data": "postalCode", "width": "15%" },
                    { "data": "phoneNo", "width": "15%" },
                    {
                        "data": "id",
                        "render": function (data) {

                            return `                       
                         <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Company/Upsert?id=${data}"
                            class="btn btn-primary mx-2"><i class="bi bi-pencil">Edit</i></a>
                            <a onclick = "Delete('/Admin/Company/DeletePost/${data}')"
                            class="btn btn-danger mx-2"><i class="bi bi-x-octagon">Delete</i></a>
                        </div>
                        `
                        }, "width": "15%"
                    }
                ]

        })
}




function Delete(url) {
    Swal.fire
        ({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        })
        .then((result) => {
            if (result.isConfirmed)
            {
                $.ajax({
                    url: url,
                    type: "Delete",
                    success: function (data) {
                      
                        if (data.success) {                          
                            toastr.success(data.message);
                            $('#tblProduct').DataTable().ajax.reload();
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                })
            }
        });

  
}