from kafka import KafkaProducer
from datetime import datetime

producer = KafkaProducer(bootstrap_servers=['localhost:19092'])

while (True):
    # input
    string = str(input())
    producer.send('msg-topic', bytes(string, 'utf-8'))
    timestamp_with_ms = datetime.now().strftime('%d/%m/%Y %H.%M.%S.%f')[:-3]
    # output
    print("Message: ", string, " sent at ", timestamp_with_ms)
