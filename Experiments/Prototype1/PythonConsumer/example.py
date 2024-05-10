from kafka import KafkaConsumer
from datetime import datetime

consumer = KafkaConsumer('msg-topic', bootstrap_servers=['34.32.47.73:30001','34.32.47.73:30002','34.32.47.73:30003'], group_id='group1')

for msg in consumer:
    message_value = msg.value.decode('utf-8')  # Assuming UTF-8 encoding
    timestamp_with_ms = datetime.now().strftime('%d/%m/%Y %H.%M.%S.%f')[:-3]
    print ("Message: ", message_value, " received at ", timestamp_with_ms)
