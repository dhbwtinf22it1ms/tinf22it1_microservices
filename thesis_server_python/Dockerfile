FROM python:3.10-alpine

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY requirements.txt /usr/src/app/

RUN pip3 install -r requirements.txt

COPY . /usr/src/app

RUN python3 -m pip install .

EXPOSE 8080

ENTRYPOINT ["python3"]
CMD ["-m", "swagger_server"]