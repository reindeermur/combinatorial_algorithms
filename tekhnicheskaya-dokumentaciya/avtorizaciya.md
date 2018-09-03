# Авторизация



Большинству команд интеграторского интерфейса требуется авторизация. Для этого команды требуют в качестве обязательного параметра так называемый авторизационный токен — массив байтов, однозначно идентифицирующий пользователя.

Кроме того, во все методы интеграторского интерфейса требуется передавать так называемый «ключ разработчика» — уникальный строковый идентификатор интегратора, который можно получить, написав запрос по адресу [diadoc-api@skbkontur.ru](mailto:diadoc-api%40skbkontur.ru). Интегратор не должен передавать свой «ключ разработчика» третьим лицам.

Для передачи авторизационной информации с каждым запросом используется стандартный HTTP-заголовк [**Authorization**](https://tools.ietf.org/html/rfc2617.html).

Схема аутентификации, используемая Диадоком, называется **DiadocAuth**. Для нее определены следующие параметры:

* _ddauth\_api\_client\_id_ — служит для передачи ключа разработчика;
* _ddauth\_token_ — служит для передачи авторизационного токена.

{% hint style="info" %}
Значения параметров отделяются от их имен символами „=“. Параметры разделяются символами „,“.
{% endhint %}

Например:

```text
Authorization: DiadocAuth
ddauth_api_client_id=testClient-8ee1638deae84c86b8e2069955c2825a,
ddauth_token=3IU0iPhuhHPZ6lrlumGz4pICEedhQ1XmlMN1Pk8z0DJ51MXkcTi6Q3CODCC4xTMsjPFfhK6XM4kCJ4JJ42hlD499/Ui5WSq6lrPwcdp4IIKswVUwyE0ZiwhlpeOwRjNrvUX1yPrxr0dY8a0w8ePsc1DG8HAlZce8a0hZiWylMqu23d/vfzRFuA==
```

Рассмотрим последовательность действий, которые требуется совершить при обращении к функциям интеграторского интерфейса Диадока.

Первым делом, клиент Диадока должен получить авторизационный токен, используя команду [Authenticate](http://api-docs.diadoc.ru/ru/latest/http/Authenticate.html).

При этом в параметре _ddauth\_api\_client\_id_ HTTP-заголовка `Authorization` требуется передать полученный в процессе регистрации ключ разработчика.

Соответствующий HTTP-запрос выглядит следующим образом:

```http
POST https://diadoc-api.kontur.ru/Authenticate HTTP/1.1
Host: diadoc-api.kontur.ru
Authorization: DiadocAuth ddauth_api_client_id=testClient-8ee1638deae84c86b8e2069955c2825a
Content-Length: 1252
Connection: Keep-Alive

<Двоичное DER-представление X.509-сертификата пользователя>
```

Успешный ответ сервера выглядит так:

```http
HTTP/1.1 200 OK
Content-Length: 598

<Двоичное DER-представление зашифрованного авторизационного токена>
```

Чтобы получить авторизационный токен, клиент должен расшифровать тело ответа команды [Authenticate](http://api-docs.diadoc.ru/ru/latest/http/Authenticate.html) при помощи закрытого ключа, соответствующего пользовательскому сертификату. Полученный массив байтов нужно закодировать в [**Base64**](https://tools.ietf.org/html/rfc3548.html) - строку.

И далее к каждому HTTP-запросу к Диадоку требуется добавлять HTTP-заголовок `Authorization` с параметрами _ddauth\_api\_client\_id=_ и _ddauth\_token=_.

Например, команда получения списка доступных пользователю ящиков транслируется в следующий HTTP-запрос:

```http
POST https://diadoc-api.kontur.ru/GetMyOrganizations HTTP/1.1
Host: diadoc-api.kontur.ru
Authorization: DiadocAuth ddauth_api_client_id=testClient-8ee1638deae84c86b8e2069955c2825a,ddauth_token=3IU0iPhuhHPZ6lrlumGz4pICEedhQ1XmlMN1Pk8z0DJ51MXkcTi6Q3CODCC4xTMsjPFfhK6XM4kCJ4JJ42hlD499/Ui5WSq6lrPwcdp4IIKswVUwyE0ZiwhlpeOwRjNrvUX1yPrxr0dY8a0w8ePsc1DG8HAlZce8a0hZiWylMqu23d/vfzRFuA==
```

Для команд, предусматривающих работу с определенным ящиком \(таких как получение и отправка сообщений\), контроль доступа к ящику работает по следующему алгоритму:

1. Из HTTP-заголовка Authorization текущего запроса извлекается значение параметра _ddauth\_token_. Это значение декодируется и из него извлекается идентификатор пользователя.

> 1. Если какое-то из вышеперечисленных действий не удалось выполнить \(например, в запросе отсутствует HTTP-заголовок `Authorization`, или в этом заголовке отсутствует параметр _ddauth\_token_, или значение этого параметра представляет собой поврежденный или просроченный токен и т.д.\), то возвращается код ошибки 401 \(Unauthorized\).
> 2. Если в запросе был указан некорректный _ddauth\_api\_client\_id_, то также возвращается код ошибки 401 \(Unauthorized\).

1. По идентификатору пользователя находится множество ящиков, к которым данный пользователь имеет доступ. Это то же самое множество, которое возвращает метод [GetMyOrganizations](http://api-docs.diadoc.ru/ru/latest/http/GetMyOrganizations.html).
2. Из параметров текущего запроса извлекается идентификатор ящика. Если идентификатор ящика не принадлежит множеству, полученному на предыдущем шаге, то возвращается код ошибки 403 \(Forbidden\).
3. Иначе доступ разрешается.

Авторизационные токены можно на некоторое время кэшировать. То есть вовсе не обязательно вызывать метод [Authenticate](http://api-docs.diadoc.ru/ru/latest/http/Authenticate.html) перед каждым обращением к методам API Диадока. Рекомендуемая стратегия заключается в получении токена на сеанс работы пользователя/программы и повторном использовании его в течение одного сеанса.

### HTTP-интерфейс

* [Authenticate](http://api-docs.diadoc.ru/ru/latest/http/Authenticate.html)
* [GetExternalServiceAuthInfo](http://api-docs.diadoc.ru/ru/latest/http/GetExternalServiceAuthInfo.html)

### Структуры данных

* [ExternalServiceAuthInfo](http://api-docs.diadoc.ru/ru/latest/proto/ExternalServiceAuthInfo.html)

