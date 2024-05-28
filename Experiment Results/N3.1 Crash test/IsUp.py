import requests
import time
import csv
from datetime import datetime

url = 'http://34.32.33.189:30080/inputservice/status'
file_path = 'Status.csv'

with open(file_path, mode='a', newline='') as file:
    writer = csv.writer(file, delimiter=';')
    
    while True:
        try:
            response = requests.get(url)
            if response.status_code == 200:
                status = 'Up'
            else:
                status = 'Down'
        except requests.RequestException:
            status = 'Down'
        
        timestamp = datetime.now().isoformat(timespec='microseconds')
        writer.writerow([timestamp, status])
        print(f"{timestamp};{status}")
        time.sleep(0.1)
