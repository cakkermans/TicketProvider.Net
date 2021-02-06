# TicketProvider.Net
.NET network libraries for DNS, SMTP and MIME written originally around 2010.

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

## Recursion

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

# Mail

## SMTP

### SMTP client

### SMTP server

## MIME

# License and attribution

The designs are largely my own, but I was definitely inspired by existing solutions around the net. Due to the age I am unable to point to my except sources, except for the RFCs.
