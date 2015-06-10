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

## อ้างอิง
- http://www.enterpriseintegrationpatterns.com/
- http://www.rabbitmq.com/getstarted.html
