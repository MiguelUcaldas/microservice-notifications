using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using ms_notifications.Models;
namespace ms_notifications.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [Route("correo-bienvenida")]
    [HttpPost]

    public async Task<ActionResult> EnviarCorreoBienvenida(ModeloCorreo datos)
    {

        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        SendGridMessage msg = this.CrearMensajeBase(datos);
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name = datos.correoDestino,
            message = "Prueba de akinmuble"
        });
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("correo enviado mensaje a la direccion" + datos.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando mensaje a la direccion" + datos.correoDestino);
        }

    }

    [Route("correo-recuperacion-clave")]
    [HttpPost]
    public async Task<ActionResult> EnviarCorreoRecuperacionClave(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name = datos.correoDestino,
            message = "Esta es su nueva clave"
        });
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("correo enviado mensaje a la direccion" + datos.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando mensaje a la direccion" + datos.correoDestino);
        }
    }

    private SenGridMessage CrearMensajeBase(ModeloCorreo datos)
    {
        
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = datos.asuntoCorreo;
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = "plain text content";
        var htmlContent = datos.asuntoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }


}
