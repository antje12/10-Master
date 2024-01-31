from kafka import KafkaProducer
from datetime import datetime
import uuid

producer = KafkaProducer(bootstrap_servers=['localhost:19092'])
direction_mapping = {'w': 'up', 's': 'down', 'a': 'left', 'd': 'right'}
id = str(uuid.uuid4())

while (True):
    # input
    user_input = str(input())
    if user_input in ['w', 's', 'a', 'd']:
        parsed_input = direction_mapping[user_input]
        producer.send('input', key=bytes(id, 'utf-8'), value=bytes(parsed_input, 'utf-8'))
    # timestamp_with_ms = datetime.now().strftime('%d/%m/%Y %H.%M.%S.%f')[:-3]
    # output
    # print("Message: ", string, " sent at ", timestamp_with_ms)
