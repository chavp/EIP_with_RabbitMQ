# EIP_with_RabbitMQ
Enterprise Integration Patterns (EIP) ด้วย RabbitMQ

รูปแบพื้นฐานของ RabbitMQ ประกอบด้วย
> 1. Hello World! ... ประกอบด้วย producer, queue, consumer โดยที่ producer ไม่สนใจ ack
> 2. Work Queues ... ประกอบด้ว producer, queue, consumer โดย producer สนใจ ack
> 3. PubSub ... ประกอบด้วย publisher, exchange ชนิด fanout และ subscriber โดย publish จะ broadcast ไปยังทุก subscriber ด้วย exchange
> 4. Routing ... ประกอบด้วย sender,  exchange ชนิด direct และ receiver โดย receiver จะผูก queue ของตนเองกับ exchange ด้วย routingKey ตามที่ตนเองสนใจ
> 5. Topics ... ประกอบด้วย sender,  exchange ชนิด topics และ receiver โดย receiver จะผูก queue ของตนเองกับ exchange ด้วย routingKey ตามที่ตนเองสนใจ แต่จะต่างจาก direct ตรงที่มี การระบุ pattern ไว้ที่ routing key ด้วยเช่น *.red.* หมายความว่า เมื่อ sender push message  ด้วย key อะไรก็แล้วแต่ ที่ตรงกลางมี red มายัง exchange ตัว exchange ก็จะ routing messgae นั้น ไปยัง queue นั้น
> 6. Remote procedure call (RPC) ... ประกอบด้วย server, queue ที่รอรับ request และ queue สำหรับ callback เพื่อตอบผลลัพย์กลับไปยัง client

----------

# Enterprise Integration Patterns (EIP)

EIP ประกอบไปด้วย 65 patterns / 6 categories

- Messaging Channels
  - Message Channel ... เชื่อต่อ applications ด้วย Message Channel
  - Point-to-Point Channel ... ต้องการส่ง message ไปยังผู้รับ แค่คนเดียวเท่านั้น 
  - Publish-Subscribe Channel ... กระจายข่าวสารเพื่อ ให้ได้รู้โดยทั่วกัน
  - Datatype Channel ... ผู้ส่งตัดสินใจ ส่งข้อมูลไปยัง channel ต่างๆตามชนิดข้อมูลที่ ผู้รับเฉพาะเจาะจงไว้
  - Invalid Message Channel  ... ผู้รับไม่รับรู้ข้อมูลที่ รับมาได้ จึงส่งข้อมูลนี้ไป Invalid Message Channel
  - Dead Letter Channel ... ผู้ส่งข้อมูลไปแล้ว แต่ผู้รับเกิดปัญหาหลุดจากระบบ  ข้อมูลนี้จะถูกส่งต่อไปที่ Dead Letter Channel
  - Guaranteed Delivery ... ถ้าระบบ messaging ล่ม รับประกันได้เลยว่า ข้อมูลทั้งหมดที่ค้างอยู่ใน channel ยังคงอยู่แน่นอน
  - Channel Adapter ... เชื่อโยง application ต่างๆที่ไม่ได้เชื่อต่อกันแบบ messaging ให้เชื่อต่อกันโดยรับส่งข้อมูลเป็น messaging ได้ ด้วย Channel Adapter
  - Messaging Bridge ... เชื่อโยงระหว่าง ระบบ messaging กับ messaging หรือ ระบบอื่นๆ กับ messaging ก็ได้หมด
  - Message Bus ... ระบบที่จะเชื่อมโยงทุกๆ application ด้วย messaging โดยกำหนด message ข้อมูลที่เป้นมาตรฐานเดียว ส่งผ่านช่องทางเดียวกันคือ Message Bus 
- Messaging Patterns
- Messaging Routing
- Message Transformation
- Messaging Endpoints
- Systems Mgmt

## อ้างอิง
- http://www.enterpriseintegrationpatterns.com/
- http://www.rabbitmq.com/getstarted.html
