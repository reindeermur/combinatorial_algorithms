---
description: >-
  Для упрощения работы с API существует SDK (C#/C++/Java/COM), скрывающий детали
  взаимодействия по HTTP и позволяющий работать с API через набор функций.
---

# Работа со счетами-фактурами



### Генерация СФ

* [CanSendInvoice](http://api-docs.diadoc.ru/ru/latest/http/CanSendInvoice.html)
* [GenerateUniversalTransferDocumentXmlForSeller](http://api-docs.diadoc.ru/ru/latest/http/utd/GenerateUniversalTransferDocumentXmlForSeller.html)
* [GenerateInvoiceDocumentReceiptXml](http://api-docs.diadoc.ru/ru/latest/http/GenerateInvoiceDocumentReceiptXml.html)
* [GetInvoiceCorrectionRequestInfo](http://api-docs.diadoc.ru/ru/latest/http/GetInvoiceCorrectionRequestInfo.html)

### Отправка СФ

* [ExtendedSignerDetails](http://api-docs.diadoc.ru/ru/latest/http/utd/ExtendedSignerDetailsV2.html)
* [PrepareDocumentsToSign](http://api-docs.diadoc.ru/ru/latest/http/PrepareDocumentsToSign.html)
* [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html)
* [PostMessagePatch](http://api-docs.diadoc.ru/ru/latest/http/PostMessagePatch.html)

### Парсинг СФ

* [ParseUniversalTransferDocumentSellerTitleXml](http://api-docs.diadoc.ru/ru/latest/http/utd/ParseUniversalTransferDocumentSellerTitleXml.html)
* [ParseUniversalCorrectionDocumentSellerTitleXml](http://api-docs.diadoc.ru/ru/latest/http/utd/ParseUniversalCorrectionDocumentSellerTitleXml.html)

### Структуры данных

* [ExtendedOrganizationInfo](http://api-docs.diadoc.ru/ru/latest/proto/utd/ExtendedOrganizationInfo.html)
* [ExtendedSigner](http://api-docs.diadoc.ru/ru/latest/proto/utd/ExtendedSigner.html)
* [ExtendedSignerDetailsToPost](http://api-docs.diadoc.ru/ru/latest/proto/utd/ExtendedSignerDetailsToPost.html)
* [UniversalCorrectionDocumentSellerTitleInfo](http://api-docs.diadoc.ru/ru/latest/proto/utd/UniversalCorrectionDocumentSellerTitleInfo.html)
* [UniversalDocumentMetadata](http://api-docs.diadoc.ru/ru/latest/proto/utd/UniversalDocumentMetadata.html)
* [UniversalTransferDocumentBuyerTitleInfo](http://api-docs.diadoc.ru/ru/latest/proto/utd/UniversalTransferDocumentBuyerTitleInfo.html)
* [UniversalTransferDocumentSellerTitleInfo](http://api-docs.diadoc.ru/ru/latest/proto/utd/UniversalTransferDocumentSellerTitleInfo.html)
* [PrepareDocumentsToSignRequest](http://api-docs.diadoc.ru/ru/latest/proto/PrepareDocumentsToSignRequest.html)
* [PrepareDocumentsToSignResponse](http://api-docs.diadoc.ru/ru/latest/proto/PrepareDocumentsToSignResponse.html)

