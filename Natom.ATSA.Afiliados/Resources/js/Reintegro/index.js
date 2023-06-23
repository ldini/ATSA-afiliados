$(document).ready(function () {

    $("#Mes, #Anio").chosen();

    $("#Mes, #Anio").change(function () {
        $('#tbl').DataTable().ajax.url("/Reintegro/ObtenerListadoIndex?mes=" + $("#Mes").val() + "&anio=" + $("#Anio").val());
        $('#tbl').DataTable().ajax.reload();
    });

    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/Reintegro/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '10%' },
            {"bSortable": true, "sWidth": '10%' },
            {"bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '26%' },
            {
                "bSortable": false, "sWidth": '34%',
                mRender: function (data, type, row) {
                    var html = "";
                    var liquidacionAnulada = row[5];
                    var transferenciaConfirmada = row[6];

                    html += '<a class="btn btn-warning btn-xs btn-ver-cambios" id="' + data + '" data-toggle="tooltip" title="Ver historial de cambios")><span class="glyphicon glyphicon-time"></span></a>';

                    if (liquidacionAnulada) {
                        html += '&nbsp;<a class="btn btn-danger btn-xs">Anulada</a>';
                    }
                    else {
                        if (!transferenciaConfirmada) {
                            html += '&nbsp;<a class="btn btn-info btn-xs btn-txt" id="' + data + '"><span class="glyphicon glyphicon-arrow-down"></span> TXT</a>';
                            html += '&nbsp;<a class="btn btn-success btn-xs btn-confirmar" id="' + data + '"><span class="glyphicon glyphicon-ok"></span> Transferencias</a>';
                            //html += '&nbsp;<a class="btn btn-default btn-xs btn-email" id="' + data + '"><span class="glyphicon glyphicon-envelope"></span></a>';
                            html += '&nbsp;<a class="btn btn-danger btn-xs btn-eliminar" id="' + data + '">Eliminar</a>';
                            html += '&nbsp;<a class="btn btn-warning btn-xs btn-editar" id="' + data + '">Editar</a>';
                        }
                        else {
                            html += '&nbsp;<a class="btn btn-success btn-xs">Confirmada</a>';
                        }
                    }

                    html += '&nbsp;<a class="btn btn-primary btn-xs btn-imprimir" id="' + data + '">Imprimir</a>';

                    return html;
                }
            }
        ]
    });

    $("#tbl").on("click", ".btn-editar", function (e) {
        var id = e.currentTarget.id;
        location.href = "/Reintegro/Edit?id=" + id.toString();
    });

    $("#tbl").on("click", ".btn-email", function (e) {
        var id = e.currentTarget.id;
        $.ajax({
            type: "POST",
            url: '/Reintegro/EnviarPorEmail',
            data: JSON.stringify({
                id: id
            }),
            dataType: "json",
            contentType: "application/json",
            success: function (data, status) {
                if (status == "success") {
                    if (!data.success) {
                        Mensajes.MostrarError(data.error);
                    }
                }
                else {
                    Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                }
            }
        });
    });

    $("#tbl").on("click", ".btn-imprimir", function (e) {
        var id = e.currentTarget.id;
        window.open("/Reintegro/Imprimir?id=" + id.toString(), "_blank");
    });

    $("#tbl").on("click", ".btn-txt", function (e) {
        var id = e.currentTarget.id;
        window.open("/Reintegro/DescargarTxt?id=" + id.toString(), "_blank");
        window.open("/Reintegro/DescargarTxt3ros?id=" + id.toString(), "_blank");
    });

    $("#tbl").on("click", ".btn-ver-cambios", function (e) {
        var id = e.currentTarget.id;
        VerHistorialCambios("ReintegroPagoHijosDiscapacitados", id);
    });
    
    $("#tbl").on("click", ".btn-confirmar", function (e) {
        var id = e.currentTarget.id;
        Mensajes.MostrarSiNo("¿Confirmar transferencias?", function () {
            $.ajax({
                type: "POST",
                url: '/Reintegro/ConfirmarTransferencias',
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
                            $("#tbl").DataTable().ajax.reload();
                        }
                        else {
                            Mensajes.MostrarError(data.error);
                        }
                    }
                    else {
                        Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                    }
                    $.unblockUI();
                }
            });
        });
    });

    $("#tbl").on("click", ".btn-eliminar", function (e) {
        var id = e.currentTarget.id;
        Mensajes.MostrarSiNo("¿Eliminar la liquidación?", function () {
            $.ajax({
                type: "POST",
                url: '/Reintegro/Eliminar',
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
                            $("#tbl").DataTable().ajax.reload();
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