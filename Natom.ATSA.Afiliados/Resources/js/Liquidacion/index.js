$(document).ready(function () {

    $("#Mes, #Anio").chosen();

    $("#Mes, #Anio").change(function () {
        $('#tbl').DataTable().ajax.url("Liquidacion/ObtenerListadoIndex?mes=" + $("#Mes").val() + "&anio=" + $("#Anio").val());
        $('#tbl').DataTable().ajax.reload();
    });

    $("#btnImprimir").click(function () {
        var mes = parseInt($("#Mes").val());
        var anio = parseInt($("#Anio").val());
        var url = "Liquidacion/ImprimirAnual?mes=" + mes.toString() + "&anio=" + anio.toString();
        window.open(url, "_blank");
    });

    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "Liquidacion/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[4, "desc"]],
        "aoColumns": [
            { "bSortable": false, "sWidth": '10%' },
            { "bSortable": false, "sWidth": '10%' },
            { "bSortable": false, "sWidth": '20%' },
            { "bSortable": false, "sWidth": '26%' },
            {
                "bSortable": true, "sWidth": '34%',
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
        location.href = "Liquidacion/Edit?id=" + id.toString();
    });

    $("#tbl").on("click", ".btn-email", function (e) {
        var id = e.currentTarget.id;
        $.ajax({
            type: "POST",
            url: 'Liquidacion/EnviarPorEmail',
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

    function SePuedenDescargarTxt(id) {
        puede = false;

        $.ajax({
            type: "POST",
            async: false,
            url: 'Liquidacion/SePuedenDescargarTxt',
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
                    else {
                        puede = true;
                    }
                }
                else {
                    Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                }
            }
        });

        return puede;
    }

    $("#tbl").on("click", ".btn-imprimir", function (e) {
        var id = e.currentTarget.id;
        window.open("Liquidacion/Imprimir?id=" + id.toString(), "_blank");
    });

    $("#tbl").on("click", ".btn-txt", function (e) {
        var id = e.currentTarget.id;
        if (SePuedenDescargarTxt(id)) {
            window.open("Liquidacion/DescargarTxt?id=" + id.toString(), "_blank");
            window.open("Liquidacion/DescargarTxt3ros?id=" + id.toString(), "_blank");
        }
    });
        
    $("#tbl").on("click", ".btn-confirmar", function (e) {
        var id = e.currentTarget.id;
        Mensajes.MostrarSiNo("¿Confirmar transferencias?", function () {
            $.ajax({
                type: "POST",
                url: 'Liquidacion/ConfirmarTransferencias',
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
                url: 'Liquidacion/Eliminar',
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