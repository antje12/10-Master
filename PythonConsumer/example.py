from kafka import KafkaConsumer
from datetime import datetime

consumer = KafkaConsumer('foo', bootstrap_servers=['localhost:19092'], group_id='group1')

for msg in consumer:
    message_value = msg.value.decode('utf-8')  # Assuming UTF-8 encoding
    timestamp_with_ms = datetime.now().strftime('%d-%m-%Y %H:%M:%S.%f')[:-3]
    print ("Message: ", message_value, " received at ", timestamp_with_ms)
