# TransformJsonToXml

Bu proje, Rest to SOAP Request işlemi için çeviri işlemini yapmaktadır.
Json veriyi Xml template ile Xml'e çevirebilen bir Helper sınıfı yazılmıştır.
Arrayler ile çalışma kapasitesi mevcuttur.
Örnek **Xml Template** ve **Json Data** ve elde edilen **Xml Result** aşağıda verilmiştir.

Xml Template üzerinde Array'i belirtmek için Json Data tarafından bir değeri belirtirken *ArrayName[].PropertyName* şeklnde belirtmek gerekir.

### Xml Template
```xml
<root>
	<books>
		<book>
			<title>{{.books[].title}}</title>
			<author>{{.books[].author}}</author>
			<static>string statik</static>
			<price>{{.books[].price}}</price>
		</book>
	</books>
	<notebook>{{.notebook}}</notebook>
	<other-static>other static value</other-static>
</root>
```
### Json Data
```json
{
  "books": [
    {
      "title": "Yeni Başlık 1",
      "author": "Yeni Yazar 1",
      "price": "39.99"
    },
    {
      "title": "Yeni Başlık 2",
      "author": "Yeni Yazar 2",
      "price": "49.99"
    },
    {
      "title": "Yeni Başlık 3",
      "author": "Yeni Yazar 3",
      "price": "49.99"
    }
  ],
  "notebook": "Yellow Notebook"
}
```
### Xml Result
```xml
<root>
  <books>
    <book>
      <title>Yeni Başlık 1</title>
      <author>Yeni Yazar 1</author>
      <static>string statik</static>
      <price>39.99</price>
    </book>
    <book>
      <title>Yeni Başlık 2</title>
      <author>Yeni Yazar 2</author>
      <static>string statik</static>
      <price>49.99</price>
    </book>
    <book>
      <title>Yeni Başlık 3</title>
      <author>Yeni Yazar 3</author>
      <static>string statik</static>
      <price>49.99</price>
    </book>
  </books>
  <notebook>Yellow Notebook</notebook>
  <other-static>other static value</other-static>
</root>
```
