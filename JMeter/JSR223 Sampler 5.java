import org.apache.kafka.clients.consumer.KafkaConsumer;

int numUsers = Integer.valueOf(vars.get("SESSION_COUNT"));

for (int i = 1; i <= numUsers; i++) {
	KafkaConsumer<String,String> kafkaConsumer = (KafkaConsumer)props.remove("consumer-"+String.valueOf(i));
	kafkaConsumer.close();
}
