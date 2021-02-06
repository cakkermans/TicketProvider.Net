# TicketProvider.Net
.NET network libraries for DNS, SMTP and IMF (mail) written originally around 2010. The overall goal of these library was to be able to deliver email right to the mail server of the recipient, avoiding the need to rely on any intermediate SMTP server.

# DNS client
This library contains a RFC 2929 compliant DNS client implementation. It offers both synchronous as well as asynchronous methods to query DNS server and to resolve hostnames to IP addresses. It was built to cover the lack of support for querying DNS servers using the .NET Framework at that time.

## Basic usage

Resolving a hostname to ip-address is done as follows:
```
var resolver = new TicketProvider.Net.Dns.Resolver();
var addresses = resolver.GetHostAddresses("www.google.nl");
```

Querying a DNS server for specific records for a hostname is done as follows:
```
var resolver = new TicketProvider.Net.Dns.Resolver();
var records = resolver.Query("www.google.nl", ResourceRecordType.A);
```

## Managing DNS server

By default the DNS client uses all locally configured DNS servers. They are retrieved using `DnsResolver.GetDnsServers()`. It is possible to specify DNS servers to use using an alternative constructor: `Resolver(IPEndPoint dnsServer)`.

DNS recursion is supported out of the box.

## Custom queries

The DNS client supports composing custom DNS queries using the `TicketProvider.Net.Dns.Request` and `TicketProvider.Net.Dns.Query` classes:

```
// Create request requesting both A and AAAA records for the host name.
var request = new Request();
request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.A, ResourceRecordClass.IN));
request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.AAAA, ResourceRecordClass.IN));
var records = resolver.GetResponse(request);
```

## Transport

Only UDP transport is implemented.

## Async support

For all methods variants are implement using the asynchronous programming model via the `Begin...()` and `End...()` naming convention.

_Nope, this was long before the introduction of the async and await keywords_

# Mail

The mail libraries consist of a message composition object model, message writer and SMTP client/server implementation. It was built to cover the lack of a complete and fully managed method to send mail from the .NET Framework at the time of writing.

Sending a message is relatively straight forward:

```
var sink = new TicketProvider.Net.Mail.Sinks.SmtpSink("smtp.server.com", 21);
var message = new TicketProvider.Net.Mail.MailMessage {
  Subject = "Your subject",
  SenderAddress = new EmailAddress("christ@vereyon.nl")
};
message.ToAddresses.Add(new EmailAddress("you@your.domain"));
sink.Send(message);
```

## Composing messages

## Deep email address validation

Using the `TicketProvider.Net.Mail.EmailValidator` class 'deep' validation of email address can be performed. This validation consists of the following steps:

1. Validate email address syntax using a simple expression
2. Check if the domain name exists using DNS
3. Check if the domain name exposes any email servers using MX records
4. Check if any the of exposed email servers are reachable
5. Attempt to deliver a message to the mailbox (but actually deposit a message)

At the time of writing (2021), I rarely see deep email address validation in the wild, but I do know that Office 365 does attempt to do something similar when delivering messages.

## SMTP

Both a SMTP client and server are implemented. A command - reply oriented architecture is followed, where every command is inplemented in a dedicated class.

### SMTP client

### SMTP server

SmtpServer creates and exposes a SmtpServerSession for every client connecting it.

### Async support

For all methods variants are implement using the asynchronous programming model via the `Begin...()` and `End...()` naming convention.

_Again, this was long before the introduction of the async and await keywords_

## IMF

A writer for the RFC5322 internet message format specification is implemented. The reader has not been implemented.

## MIME

Some basic scaffolding for MIME support can be found 

## Message sinks

Various sinks for transmitting messages to can be used: SMTP, file and stream. They all need to implement the `TicketProvider.Net.Mail.Sinks.IMessageSink` interface.

# License and attribution

The designs are largely my own, but I was definitely inspired by existing solutions around the net. Due to the age I am unable to point to my except sources, except for the RFCs.

Licences under the MIT license
