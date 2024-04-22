import org.apache.kafka.clients.consumer.KafkaConsumer;
import java.time.Duration;

String brokers = vars.get("KAFKA_BROKERS");
String topic = vars.get("KAFKA_TOPIC");
int numUsers = Integer.valueOf(vars.get("SESSION_COUNT"));
int messageLength = Integer.valueOf(vars.get("MESSAGE_LENGTH"));

Properties kafkaProps = new Properties();
kafkaProps.setProperty("bootstrap.servers", brokers);
kafkaProps.setProperty("enable.auto.commit", "true");
kafkaProps.setProperty("auto.commit.interval.ms", "1000");
kafkaProps.setProperty("key.deserializer", "org.apache.kafka.common.serialization.StringDeserializer");
kafkaProps.setProperty("value.deserializer", "org.apache.kafka.common.serialization.StringDeserializer");
kafkaProps.setProperty("fetch.max.wait.ms", "100");
kafkaProps.setProperty("fetch.min.bytes", String.valueOf((numUsers * messageLength) / 2));

for (int i = 1; i <= numUsers; i++) {
	kafkaProps.setProperty("group.id", "user-" + String.valueOf(i));
	kafkaProps.setProperty("client.id", "consumer-client-"+String.valueOf(i));
	KafkaConsumer<String, String> consumer = new KafkaConsumer<String, String>(kafkaProps); 
	consumer.subscribe(Arrays.asList(topic));
	// immediate poll to join the group and rebalance before producer is executed
	consumer.poll(Duration.ofMillis(100));

	props.put("consumer-"+String.valueOf(i), consumer);
}
