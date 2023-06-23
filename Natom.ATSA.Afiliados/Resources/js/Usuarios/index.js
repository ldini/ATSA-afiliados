$(document).ready(function () {
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/Usuarios/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '40%' },
            {
                "bSortable": false, "sWidth": '20%',
                mRender: function (data, type, row) {
                    var html = "";
                    html += '&nbsp;<a class="btn btn-primary btn-xs btn-editar" id="' + data + '">Editar</a>';
                    html += '&nbsp;<a class="btn btn-danger btn-xs btn-eliminar" id="' + data + '">Eliminar</a>';
                    
                    return html;
                }
            }
        ],
        "fnDrawCallback": function () {
            $(this).find('[data-toggle="tooltip"]').tooltip();
        }
    });

    $("#tbl").on("click", ".btn-editar", function (e) {
        var id = e.currentTarget.id;
        location.href = "/Usuarios/Edit?id=" + id.toString();
    });

    $("#tbl").on("click", ".btn-eliminar", function (e) {
        var id = e.currentTarget.id;
        Mensajes.MostrarSiNo("¿Eliminar el usuario?", function () {
            $.ajax({
                type: "POST",
                url: '/Usuarios/Eliminar',
                data: JSON.stringify({
                    id: id
                }),
                dataType: "json",
                contentType: "application/json",
                beforeSend: function (xhr) {
                    $.blockUI({ message: '<h4>Procesando...</h4>' });
                },
                success: function (data, status) {
                    if (status == "success") {
                        if (data.success) {
                            $.unblockUI();
                            $(".table").DataTable().ajax.reload();
                        }
                        else {
                            Mensajes.MostrarError(data.error);
                        }
                    }
                    else {
                        Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                        $.unblockUI();
                    }
                }
            });
        }, undefined);
    });
});