import java.util.concurrent.TimeUnit;

String user = String.valueOf(ctx.getThreadNum() + 1);
def latencyStr = props.get("consumer-"+user+".receive.latency")
prev.setSampleLabel("Consume Kafka message for user - receive latency");
org.apache.commons.lang3.reflect.FieldUtils.writeField(prev, "elapsedTime", Long.valueOf(latencyStr), true);
