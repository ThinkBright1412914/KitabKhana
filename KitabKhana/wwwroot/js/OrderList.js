var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        LoadData("inprocess");
    }
    else
    {
        if (url.includes("pending")) {
            LoadData("pending");
        }
        else
        {
            if (url.includes("completed")) {
                LoadData("completed");
            }
            else
            {
                if (url.includes("approved")) {
                    LoadData("approved");
                }
                else
                {
                    LoadData("all");
                }
            }
        } 
    }
       
    

})

function LoadData(status) {
    var dataTable =
        $('#tblProduct').DataTable({
            "ajax": {
                "url": "/Admin/Order/GetAll?status=" + status,
            },
            "columns":
                [
                    { "data": "id", "width": "5%" },
                    { "data": "name", "width": "15%" },
                    { "data": "phoneNo", "width": "15%" },
                    { "data": "applicationUser.email", "width": "15%" },
                    { "data": "orderStatus", "width": "15%" },
                    { "data": "orderTotal", "width": "10%" },
                    {
                        "data": "id",
                        "render": function (data) {

                            return `                       
                         <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Order/Details?orderId=${data}"
                            class="btn btn-success mx-2"><i class="bi bi-pencil"></i></a>
                           
                        </div>
                        `
                        }, "width": "10%"
                    }
                ]

        })
}




