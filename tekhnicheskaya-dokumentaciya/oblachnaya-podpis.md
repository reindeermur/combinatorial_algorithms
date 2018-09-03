# Облачная подпись



Подписание облачной ЭП осуществляется в два этапа.

Во-первых, надо вызвать метод [CloudSign](http://api-docs.diadoc.ru/ru/latest/http/CloudSign.html) и дождаться его успешного завершения периодически вызывая метод [CloudSignResult](http://api-docs.diadoc.ru/ru/latest/http/CloudSignResult.html).

В результате успешного завершения этого этапа, вы получите token \(идентификатор операции с облачной ЭП\), а на телефон пользователя будет отправлено SMS-сообщение с кодом подтверждения операции.

Во-вторых, надо вызывать метод [CloudSignConfirm](http://api-docs.diadoc.ru/ru/latest/http/CloudSignConfirm.html) и передать в него полученный token и SMS-код. Затем, вызывая метод [CloudSignConfirmResult](http://api-docs.diadoc.ru/ru/latest/http/CloudSignConfirmResult.html), дождаться окончания операции и получить, либо подписи, либо код ошибки.

Ниже приведен пример кода на C\#

```csharp
//Загружаем данные на полку
var nameOnShelf1 = api.UploadFileToShelf(authToken, Encoding.UTF8.GetBytes("TEST document 1"));
var nameOnShelf2 = api.UploadFileToShelf(authToken, Encoding.UTF8.GetBytes("TEST document 2"));

//Подписание
var request = new CloudSignRequest {
    Files = {
        new CloudSignFile{ FileName = "File 1.txt", Content = new Content_v2{ NameOnShelf = nameOnShelf1 } },
        new CloudSignFile{ FileName = "File 2.txt", Content = new Content_v2{ NameOnShelf = nameOnShelf2 } },
    }
};
var aResult = api.CloudSign(authToken, request, cloudCertificateThumbprint);
var cloudSignResult = api.WaitCloudSignResult(authToken, aResult.TaskId);

//Запрашиваем у пользователя SMS-код подтверждения confirmationCode
//Подтверждаем операцию подписи
aResult = api.CloudSignConfirm(authToken, cloudSignResult.Token, confirmationCode);
var cloudSignConfirmResult = api.WaitCloudSignConfirmResult(authToken, aResult.TaskId);

//Получаем подписи
Assert.AreEqual(2, cloudSignConfirmResult.Signatures.Count);
```

### HTTP-интерфейс

* [CloudSign](http://api-docs.diadoc.ru/ru/latest/http/CloudSign.html)
* [CloudSignConfirm](http://api-docs.diadoc.ru/ru/latest/http/CloudSignConfirm.html)
* [CloudSignConfirmResult](http://api-docs.diadoc.ru/ru/latest/http/CloudSignConfirmResult.html)
* [CloudSignResult](http://api-docs.diadoc.ru/ru/latest/http/CloudSignResult.html)
* [AutoSignReceipts](http://api-docs.diadoc.ru/ru/latest/http/AutoSignReceipts.html)
* [AutosignReceiptsResult](http://api-docs.diadoc.ru/ru/latest/http/AutoSignReceiptsResult.html)

### Структуры данных

* [AsyncMethodResult](http://api-docs.diadoc.ru/ru/latest/proto/AsyncMethodResult.html)
* [CloudSignRequest](http://api-docs.diadoc.ru/ru/latest/proto/CloudSignRequest.html)
* [CloudSignConfirmResult](http://api-docs.diadoc.ru/ru/latest/proto/CloudSignConfirmResultDTO.html)
* [CloudSignResult](http://api-docs.diadoc.ru/ru/latest/proto/CloudSignResultDTO.html)
* [AutosignReceiptsResult](http://api-docs.diadoc.ru/ru/latest/proto/AutosignReceiptsResult.html)

