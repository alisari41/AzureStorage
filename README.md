
# Azure Storage, Azure Functions, SignalR ile Resim yazısı ekleme uygulaması

Bu uygulamada Azure portaldan Azure Storage servisleri kullanılmıştır. Azure File Storage, Blob Storage, Table Storage, Queue Storage servisleri kullanılmaktadır. Uygulamada Table Storage NoSql (ilişkisel olmayan) veritabanları kullanılmıştır. Uygulamada resim ekleme işlemi gerçekleştirebilirsiniz ve ayrıcı istediğiniz resimlere Resim Yazısı ekleyebilirsiniz. Resim yazısı ekleme işlemi için seçilen resim yazısı girildikten sonra girilen resim yazısı ile beraber queue/kuyruğa giriyor. Azure Function ile gelen mesaj alınarak işleme koyuluyor. İşlem tamamlandıktan sonra Client'ta haber veriyor. Eski resimler ve yeni resim yazısı eklenmiş resimler ayrı ayrı tutuluyor. Binebi küçük bir microservice diyebiliriz. 
    
![image](https://user-images.githubusercontent.com/81421228/156018815-9e2252fc-8140-46b8-a6ff-677f9a6f1bf5.png)

## Demo

- Table Storage ile tabloya veri ekleme işlemi
![image](https://user-images.githubusercontent.com/81421228/156060033-a4a64372-3008-4a42-9fcc-ce7792a31b51.png)

- Veri güncelleme yapılabilmektedir ve istediğiniz veriyi silebilinebilmektedir.
![image](https://user-images.githubusercontent.com/81421228/156060441-56fcb37f-610c-45e3-8893-4328de3bad31.png)

- Öncelikle Resimler ekliyelim
![image](https://user-images.githubusercontent.com/81421228/156058127-241fd9b3-096f-4e61-9eb0-6e5710ae3e05.png)

- Resim yazısı eklenecek resimleri seçiyoruz ve eklenecek yazı giriliyor.
![image](https://user-images.githubusercontent.com/81421228/156058385-27b5fdb9-9746-41e7-a7d2-ef1662fbbd7b.png)

- Resimler queue/kuyruğa gitti kuyruktan azure fuction projesi ile işleniyor. Bu mesajı client'a veriliyor.
![image](https://user-images.githubusercontent.com/81421228/156058520-aa916073-6ba2-409b-8101-41492ae803b0.png)

- İşlemler yapıldıktan sonra Client ile iletişim kurularak bilgilendirme yapılıyor
![image](https://user-images.githubusercontent.com/81421228/156058580-3fd8fe3a-f590-4504-a328-287e9a57781f.png)

- Resim yazısı eklediğimiz resimler
![image](https://user-images.githubusercontent.com/81421228/156058635-0e43b934-0d01-4489-9908-0b86c281750c.png)

  
