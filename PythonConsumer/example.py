from kafka import KafkaConsumer
from datetime import datetime

consumer = KafkaConsumer('foo', bootstrap_servers=['localhost:19092'], group_id='group1')

for msg in consumer:
    timestamp_with_ms = datetime.now().strftime('%d-%m-%Y %H:%M:%S.%f')[:-3]
    print ("Message: ", msg.value, " received at ", timestamp_with_ms)
