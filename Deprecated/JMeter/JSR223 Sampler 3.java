import org.apache.kafka.clients.producer.Producer;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;

import java.util.concurrent.TimeUnit;

String brokers = vars.get("KAFKA_BROKERS");
String topic = vars.get("KAFKA_TOPIC");
int msgLength = Integer.valueOf(vars.get("MESSAGE_LENGTH"));
String user = String.valueOf(ctx.getThreadNum() + 1);
String msg = (String) props.get("message");

Properties kafkaProps = new Properties();
kafkaProps.put("bootstrap.servers", brokers);
kafkaProps.put("acks", "1");
kafkaProps.put("linger.ms", 0);
kafkaProps.put("request.timeout.ms", 5000);
kafkaProps.put("delivery.timeout.ms", 30000);
kafkaProps.put("batch.size", msgLength);
kafkaProps.put("buffer.memory", msgLength*3);
kafkaProps.put("key.serializer", "org.apache.kafka.common.serialization.StringSerializer");
kafkaProps.put("value.serializer", "org.apache.kafka.common.serialization.StringSerializer");
kafkaProps.put("client.id", "producer-client-"+user);

long beforeSend = System.nanoTime();
Producer<String, String> producer = new KafkaProducer<>(kafkaProps);
try {
	producer.send(new ProducerRecord<String, String>(topic, user, msg)).get();
	long afterSend = System.nanoTime();
	long timeToSend = TimeUnit.NANOSECONDS.toMillis(afterSend - beforeSend);
	producer.send(new ProducerRecord<String, String>(topic, user, "producer-time:"+String.valueOf(timeToSend))).get();
} finally {
	producer.close();
}