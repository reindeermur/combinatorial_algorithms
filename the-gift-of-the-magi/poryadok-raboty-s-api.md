# Порядок работы с API

В данном разделе описаны только базовые функции, предоставляемые посредством программного интеграторского веб-интерфейса Диадока для организации документооборота. Дополнительные возможности описаны в соответствующих разделах. Полный список команд API всегда можно посмотреть в технической документации.

### Аутентификация

Любой сеанс работы клиента API начинается с прохождения процедуры аутентификации, в ходе которой программный клиент от имени пользователя получает авторизационный токен, однозначно идентифицирующий этого пользователя и дающий клиенту право читать и писать в ящики Диадока, к которым данный пользователь имеет доступ.

Пользователь может аутентифицироваться в системе либо по сертификату электронной подписи формата X.509, либо по паре логин/пароль.

В результате аутентификации формируется специальный авторизационный токен, дающий право доступа к Диадоку посредством интеграционного API в течение некоторого интервала времени \(порядка нескольких часов\).

Полученный авторизационный токен в дальнейшем должен передаваться в качестве параметра во все методы API, выполняющие какие-либо операции в контексте конкретного ящика, чтобы авторизовать доступ пользователя к этому ящику. Кроме того, во все методы API клиент должен передавать так называемый «ключ разработчика», однозначно идентифицирующий автора данного интеграционного решения.

{% hint style="info" %}
Более подробно механизм аутентификации и авторизации в Диадоке описан [здесь](http://api-docs.diadoc.ru/ru/latest/Authorization.html).
{% endhint %}

### Передача документа

Каждый передаваемый через Диадок документ представляется в виде структуры [Entity](http://api-docs.diadoc.ru/ru/latest/proto/Entity%20message.html), которая может содержать как описание документа пользователя, так и описание служебного документа системы \(комментарии к документу, подписи к документу и т.п.\).

Соответственно, набор взаимосвязанных пользовательских и служебных документов \(т.е. вся цепочка документооборота, связанная с конкретным набором пользовательских документов, передаваемых через Диадок\) образует сообщение \(объект [Message](http://api-docs.diadoc.ru/ru/latest/proto/Message.html)\).

Каждый документооборот может охватывать либо две организации \(в этом случае естественным образом по направлению передачи первого документа в цепочке выделяются организация-отправитель и организация-получатель\), либо одну организацию \(в случае внутреннего документооборота внутри конкретной организации\).

Соответственно, в рамках документооборота между двумя организациями формируются две цепочки: в ящике отправителя и в ящике получателя.

Документы, которые были сформированы и отправлены, отображаются в обеих цепочках. Документы, которые были только сформированы, но не отправлены \(см. возможности метода [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html) ниже\), отображаются в соответствующей цепочке \(либо только в ящике отправителя, либо только в ящике получателя\).

Подготовка и отправка исходящих сообщений осуществляется при помощи метода [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html), которому на вход передается структура [MessageToPost](http://api-docs.diadoc.ru/ru/latest/proto/MessageToPost.html). Эта структура содержит идентификаторы ящиков участников документооборота \(ящики отправителя и получателя сообщения\) и собственно набор отправляемых документов.

В качестве ящика отправителя клиент может указывать только «свой» ящик, то есть ящик, к которому он может получить доступ при помощи имеющегося у него авторизационного токена.

В результате вызова метода [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html) формируется новая цепочка документооборота, связывающая ящики отправителя и получателя.

В ящике отправителя информация о сформированной цепочке появляется в момент вызова метода [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html) \(соответственно, формируется событие о появлении документа, информацию о событиях см. ниже\).

Информация о новой цепочке документооборота и связанных с ней документах в ящике получателя, вообще говоря, появится с некоторой задержкой, связанной с асинхронной передачей информации из ящика отправителя в ящик получателя.

То есть успешный вызов метода PostMessage гарантирует лишь появление исходящего сообщения в ящике отправителя; в ящике получателя сообщение и соответствующее событие могут появиться с некоторой задержкой.

Метод [PostMessage](http://api-docs.diadoc.ru/ru/latest/http/PostMessage.html) можно также использовать для формирования на сервере сообщений, содержащих отправляемые документы без подписей к ним \(см. флаг IsDraft структуры [MessageToPost](http://api-docs.diadoc.ru/ru/latest/proto/MessageToPost.html); если он выставлен в true, то сообщение будет загружено на сервер, но задание на доставку сообщения его получателю формироваться не будет\). В этом случае для формирования подписей к документам и отправки сообщения следует использовать метод [SendDraft](http://api-docs.diadoc.ru/ru/latest/http/SendDraft.html).

### Дополнение документа

Уже сформированные цепочки документооборота можно дополнять служебными документами при помощи метода [PostMessagePatch](http://api-docs.diadoc.ru/ru/latest/http/PostMessagePatch.html), которому на вход передается структура [MessagePatchToPost](http://api-docs.diadoc.ru/ru/latest/proto/MessagePatchToPost.html).

Эта структура содержит идентификатор цепочки документооборота, которую следует дополнить новым документом, и идентификатор ящика, с которым эта цепочка связана \(если в документооборот вовлечено две организации, то в ящике второй стороны цепочка документооборота также будет обновлена; обновление производится асинхронно\).

Клиент должен дополнять цепочку документооборота через «свой» ящик, то есть через тот ящик, к которому у него есть доступ.

Если загружаемый документ имеет большой размер \(больше 100Кб\), то для загрузки такого документа в Диадок следует пользоваться сервисом «полки документов».

В этом случае документ сначала загружается на сервер Диадока с помощью серии вызовов [ShelfUpload](http://api-docs.diadoc.ru/ru/latest/http/ShelfUpload.html), а затем в структурах [MessageToPost](http://api-docs.diadoc.ru/ru/latest/proto/MessageToPost.html) и [MessagePatchToPost](http://api-docs.diadoc.ru/ru/latest/proto/MessagePatchToPost.html) можно ссылаться на уже загруженный документ. Такой подход позволяет повысить скорость и надежность загрузки.

### Получение документа

Для получения текущего состояния конкретной цепочки документооборота можно использовать метод [GetMessage](http://api-docs.diadoc.ru/ru/latest/http/GetMessage.html), который возвращает все документы, составляющие данную цепочку, агрегированные в одну структуру [Message](http://api-docs.diadoc.ru/ru/latest/proto/Message.html).

Отметим, что структура [Message](http://api-docs.diadoc.ru/ru/latest/proto/Message.html) может содержать документы, сформированные в разное время разными организациями \(например, в одну такую структуру могут попасть исходящий документ одной организации и подпись к этому документу, поставленная представителем другой организации\).

Для того, чтобы получить содержимое конкретного документа в цепочке документооборота, следует взять идентификаторы из полей boxId и messageId структуры [Message](http://api-docs.diadoc.ru/ru/latest/proto/Message.html) и идентификатор документа entityId из соответствующей структуры [Entity](http://api-docs.diadoc.ru/ru/latest/proto/Entity%20message.html), а затем воспользоваться методом [GetEntityContent](http://api-docs.diadoc.ru/ru/latest/http/GetEntityContent.html).

Таким образом, каждый ящик в Диадоке может изменяться лишь одним из двух способов:

* в ящике формируется новая цепочка документооборота;
* дополняется уже существующая в ящике цепочка документооборота.

То есть вся уже существующая в ящике информация не может быть изменена, она может быть лишь дополнена. Соответственно, все модификации ящика естественным образом упорядочиваются хронологически, и можно говорить о «событиях», связанных с конкретным ящиком:

* событие о формировании новой цепочки документооборота;
* событие о добавлении документа к уже имеющейся цепочки документооборота.

Чтобы получить информацию о новых событиях следует использовать метод [GetNewEvents](http://api-docs.diadoc.ru/ru/latest/http/GetNewEvents.html). Этот метод предоставляет доступ к упорядоченному хронологически потоку всех Событий \([BoxEvent](http://api-docs.diadoc.ru/ru/latest/proto/BoxEvent.html)\), «происходящих» в заданном ящике.
