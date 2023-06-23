$(document).ready(function () {

    $("#PermisoId").chosen();

    $("form").validate({
        rules: {
            Nombre: {
                required: true,
                minlength: 3,
                maxlength: 50
            },
            Apellido: {
                required: true,
                minlength: 3,
                maxlength: 50
            },
            Email: {
                required: true,
                minlength: 3,
                maxlength: 150,
                email: true
            },
            PermisoId: {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            if (element.is("select")) {
                error.insertAfter("#" + element.attr("id") + "_chosen");
            } else {
                error.insertAfter(element);
            }
        }
    });
});

function Grabar() {
    if (!$("form").valid()) {
        return;
    }

    var obj = {
        UsuarioId: $("#UsuarioId").val(),
        Nombre: $("#Nombre").val(),
        Apellido: $("#Apellido").val(),
        Email: $("#Email").val()
    };

    $.ajax({
        type: "POST",
        url: '/Usuarios/Grabar',
        data: JSON.stringify({
            usuario: obj
        }),
        dataType: "json",
        contentType: "application/json",
        beforeSend: function (xhr) {
            $.blockUI({ message: '<h4>Procesando...</h4>' });
        },
        success: function (data, status) {
            if (status == "success") {
                if (data.success) {
                    location.href = "/Usuarios";
                }
                else {
                    Mensajes.MostrarError(data.error);
                }
                $.unblockUI();
            }
            else {
                Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                $.unblockUI();
            }
        }
    });

}