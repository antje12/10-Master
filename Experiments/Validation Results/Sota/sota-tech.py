import matplotlib.pyplot as plt
from matplotlib.ticker import MaxNLocator

categories = {
    "Orchestration": {"Docker": 4, "K8s": 4, "Traefik": 1, "Istio": 1, "Consul": 1},
    "Communication": {"REST": 7, "Msg Broker": 5, "RPC": 3, "WebSocket": 2},
    "Message Brokers": {"Kafka": 4, "RabbitMQ": 4, "ActiveMQ": 2},
    "Data Storage": {"Cassandra": 2, "MongoDB": 1, "Couchbase": 1, "Redis": 1, "SparQL": 1, "MySQL": 1}
}

fig, axes = plt.subplots(ncols=len(categories), figsize=(15, 5))
fig.tight_layout(pad=5.0)

for ax, (category, data) in zip(axes, categories.items()):
    names = list(data.keys())
    values = list(data.values())
    
    ax.bar(names, values, color='lightblue')
    ax.set_title(category)
    ax.set_xlabel('Technologies')
    ax.set_ylabel('Count')
    ax.tick_params(axis='x', rotation=45)
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))

plt.show()
