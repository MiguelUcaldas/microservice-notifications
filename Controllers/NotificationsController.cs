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
    [Route("correo")]
    [HttpPost]

    public async Task<ActionResult> EnviarCorreo(ModeloCorreo datos){

        var apiKey = Environment.GetEnvironmentVariable("TU_CLAVE_API");
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress("rojas_b23@hotmail.com","miguel rojas");
        var subject = datos.asuntoCorreo;
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = "plain text content";
        var htmlContent = datos.asuntoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("correo enviado mensaje a la direccion" + datos.correoDestino);
        }
        else{
            return BadRequest("Error enviando mensaje a la direccion" + datos.correoDestino);
        }

    } 
} 
