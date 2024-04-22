import org.apache.kafka.clients.consumer.KafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.apache.kafka.clients.consumer.ConsumerRecord;

import java.time.Duration;
import java.util.concurrent.TimeUnit;

String user = String.valueOf(ctx.getThreadNum() + 1);
String brokers = vars.get("KAFKA_BROKERS");
String topic = vars.get("KAFKA_TOPIC");
int msgLength = Integer.valueOf(vars.get("MESSAGE_LENGTH"));

boolean msgFound = false;
Long producerTime = null;
KafkaConsumer<String, String> consumer = props.get("consumer-"+user);

long startPollTime = System.nanoTime();
do {
	records = consumer.poll(Duration.ofMillis(100));
	for (ConsumerRecord<String, String> record : records) {
//			System.out.printf("offset = %d, key = %s, value = %s\n", record.offset(), record.key(), record.value());
		if (record.key().equals(user)) {
			if (record.value().startsWith("producer-time:")) {
				producerTime = Long.valueOf(record.value().substring("producer-time:".length()));
			} else {
				msgFound = true;
			}
			if (msgFound && producerTime != null) {
				break;
			}
		}		
	}	
} while (msgFound == false || producerTime == null);

long endPollTime = System.nanoTime();
long timeToFindKey = TimeUnit.NANOSECONDS.toMillis(endPollTime - startPollTime);
long latency = producerTime+timeToFindKey;
props.put("consumer-"+user+".receive.latency", String.valueOf(latency));
