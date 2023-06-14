using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notifications.Models;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace ms_notifications.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [Route("correo-bienvenida")]
    [HttpPost]
    public async Task<ActionResult> EnviarCorreoBienvenida(ModeloCorreo datos){
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new{
            name=datos.nombreDestino,
            message="Bienvenido a la comunidad de la inmobiliaria"
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }else{
            return BadRequest("Error enviando el mensaje a la dirección " + datos.correoDestino);
        }
    }

    [Route("correo-recuperacion-clave")]
    [HttpPost]
    public async Task<ActionResult> EnviarCorreoRecuperacionClave(ModeloCorreo datos){
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new{
            name=datos.nombreDestino,
            message="Esta es su nueva clave... no la comparta."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }else{
            return BadRequest("Error enviando el mensaje a la dirección " + datos.correoDestino);
        }
    }

    
    [Route("enviar-correo-2fa")]
    [HttpPost]
    public async Task<ActionResult> EnivarCorreo2fa(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("TwoFA_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name = datos.correoDestino,
            message = datos.contenidoCorreo,
            asunto = datos.asuntoCorreo
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

    private SendGridMessage CrearMensajeBase(ModeloCorreo datos)
    {   
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = datos.asuntoCorreo;
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = datos.contenidoCorreo;
        var htmlContent = datos.asuntoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }

    private SendGridMessage CrearMensajeBaseContacto(ModeloContacto datos)
    {   
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var Subject = "Contacto Akinmueble";
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = datos.contenidoCorreo;
        var htmlContent = datos.asuntoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, Subject, plainTextContent, htmlContent);
        return msg;
    }

    private SendGridMessage CrearMensajeBaseSolicitud(ModeloSolicitud datos)
    {   
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var Subject = "Solicitud Akinmueble";
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = datos.propiedadId;
        var htmlContent = datos.asuntoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, Subject, plainTextContent, htmlContent);
        return msg;
    }

    //Envío de SMS
    [Route("enviar-sms")]
    [HttpPost]
    public async Task<ActionResult> EnviarSMSNuevaClave(ModeloSms datos)
    {
        var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_AWS");
        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY_AWS");
        var client = new AmazonSimpleNotificationServiceClient(accessKey, secretKey, RegionEndpoint.USEast1);
        var messageAttributes = new Dictionary<string, MessageAttributeValue>();
        var smsType = new MessageAttributeValue
        {
            DataType = "String",
            StringValue = "Transactional"
        };

        messageAttributes.Add("AWS.SNS.SMS.SMSType", smsType);

        PublishRequest request = new PublishRequest
        {
            Message = datos.contenidoMensaje,
            PhoneNumber = datos.numeroDestino,
            MessageAttributes = messageAttributes
        };
        try
        {
            await client.PublishAsync(request);
            return Ok("Mensaje enviado");
        }
        catch
        {
            return BadRequest("Error enviando el sms");
        }
    }

    [Route("enviar-sms-2fa")]
    [HttpPost]
    public async Task<ActionResult> EnviarSMS2fa(ModeloSms datos)
    {
        var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_AWS");
        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY_AWS");
        var client = new AmazonSimpleNotificationServiceClient(accessKey, secretKey, RegionEndpoint.USEast1);
        var messageAttributes = new Dictionary<string, MessageAttributeValue>();
        var smsType = new MessageAttributeValue
        {
            DataType = "String",
            StringValue = "Transactional"
        };

        messageAttributes.Add("AWS.SNS.SMS.SMSType", smsType);

        PublishRequest request = new PublishRequest
        {
            Message = datos.contenidoMensaje,
            PhoneNumber = datos.numeroDestino,
            MessageAttributes = messageAttributes
        };
        try
        {
            await client.PublishAsync(request);
            return Ok("Mensaje enviado");
        }
        catch
        {
            return BadRequest("Error enviando el sms");
        }
    }

    [Route("enviar-correo-contact")]
    [HttpPost]
    public async Task<ActionResult> FormularioContacto(ModeloContacto datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey); // crear el cliente

        SendGridMessage msg = this.CrearMensajeBaseContacto(datos); 
        msg.SetTemplateId(Environment.GetEnvironmentVariable("CONTACT_FORM_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new{
            nombreDestino = datos.nombreDestino,
            nombre = datos.nombre,
            celular = datos.celular,
            correo = datos.correo,
            titulo = datos.asuntoCorreo,
            mensaje = datos.contenidoCorreo
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }else{
            return BadRequest("Error enviando el mensaje a la dirección: " + datos.correoDestino);
        }
        
    }

    [Route("enviar-correo-solicitud")]
    [HttpPost]
    public async Task<ActionResult> FormularioSolicitud(ModeloSolicitud datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey); // crear el cliente

        SendGridMessage msg = this.CrearMensajeBaseSolicitud(datos); 
        msg.SetTemplateId(Environment.GetEnvironmentVariable("REQUEST_FORM_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new{
            nombreDestino = datos.nombreDestino,
            nombre = datos.nombre,
            celular = datos.celular,
            correo = datos.correo,
            titulo = datos.asuntoCorreo,
            propiedadId = datos.propiedadId
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }else{
            return BadRequest("Error enviando el mensaje a la dirección: " + datos.correoDestino);
        }

    }
}
