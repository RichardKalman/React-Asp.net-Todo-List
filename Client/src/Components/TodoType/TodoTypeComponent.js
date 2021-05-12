import { Col, Container, Row, Card, ListGroup } from 'react-bootstrap';
  import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
  import { faTimes, faPlus, faArrowUp, faArrowDown } from '@fortawesome/free-solid-svg-icons';
  import _ from 'lodash';
  import { useState, useEffect } from 'react';
  import NewTodoTypeModalComponent from './NewTodoTypeModalComponent';
  import DeleteTodoTypeComponent from './DeleteTodoTypeComponent';
  
  export function TodoTypeComponent() {
    const [modalDeleteShow, setModalDeleteShow] = useState(false);
    const [modalNewShow, setModalNewShow] = useState(false);
    const [getTodoTypes, setTodoType] = useState({});
    const [getSelected, setSelected] = useState({});
  
    const allTodoTypeLoad = async () => {
      let types = await fetch(`${process.env.REACT_APP_API_URL}/api/todotype`).then((res) => res.json());
  
      types = types.map((tt) => ({ ...tt, todoSize: 0 })).reduce((aggregalt, tt) => ({ ...aggregalt, [tt.order]: tt }), {});
  
      if (!_.isEmpty(types)) {
        const todos = await fetch(`${process.env.REACT_APP_API_URL}/api/todo`).then((res) => res.json());
        todos.forEach((data) => {
          types[data.type.order].todoSize++;
        });
      }
      setTodoType(types);
    };
  
    const sendOrder = async (item, fromindex, toindex) => {
      const send = {
        "id": item.id,
        "sourceindex": fromindex - 1,
        "destinationindex": toindex - 1,
  
      };
  
      const requestOptions = {
        method: 'PUT',
        body:  JSON.stringify(send),
        headers: {
          'Content-Type': 'application/json'
        },
      };
      try {
        fetch(`${process.env.REACT_APP_API_URL}/api/todotype/rowSort`, requestOptions);
      } catch (error) {
        console.error(error);
      }
    };
  
    const orderTodoType = (item, fromindex, toindex) => {
      sendOrder(item, fromindex, toindex);
  
      setTodoType((prev) => {
        prev = { ...prev };
  
        const tempprevarray = Object.keys(prev).map((key) => getTodoTypes[key]);// objektból arrayt csinálunk
        tempprevarray[fromindex - 1].order = toindex;
        tempprevarray[toindex - 1].order = fromindex;
        prev = tempprevarray.reduce((aggregalt, tt) => ({ ...aggregalt, [tt.order]: tt }), {});// visszaalakítjuk objectre
  
        return prev;
      });
    };
  
    useEffect(() => {
      allTodoTypeLoad();
    }, []);
  
    return (
      <Container>
        <Row>
          <Col>
            <Card className="bg-dark color">
              <Card.Header>
                Todo táblák kezelése
                <span className="float-right">
                  <FontAwesomeIcon
                    icon={faPlus}
                    className="add-fa"
                    onClick={() => {
                      setModalNewShow(true);
                    }}
                  />
                </span>
              </Card.Header>
              <Card.Body className="p-3 ">
                <Card.Title>Táblák</Card.Title>
                <ListGroup>
                  {_.map(getTodoTypes, (item, index) => (
                    <ListGroup.Item key={index.toString()} className="bg-dark color border-color">
                      <span className="font-weight-bold">{item.name}</span>
                      <span>
                        . feladatok száma:
                        {item.todoSize}
                      </span>
                      <span className="float-right ">
                        {item.order !== 1 && (
                            <FontAwesomeIcon
                            icon={faArrowUp}
                            className="up-down-fa mr-1"
                            onClick={() => {
                                orderTodoType(item, item.order, item.order - 1);
                            }}
                            />
                        )}
                        {item.order !== Object.keys(getTodoTypes).length && (
                            <FontAwesomeIcon
                            icon={faArrowDown}
                            className="up-down-fa mr-1"
                            onClick={() => {
                                orderTodoType(item, item.order, item.order + 1);
                            }}
                            />
                        )}
                        <FontAwesomeIcon
                          icon={faTimes}
                          className="delete-fa mr-1"
                          onClick={() => {
                            setModalDeleteShow(true);
                            setSelected(item);
                          }}
                        />
                      </span>
                    </ListGroup.Item>
                  ))}
  
                </ListGroup>
  
              </Card.Body>
            </Card>
          </Col>
        </Row>
        <DeleteTodoTypeComponent getTodoTypes={getTodoTypes} selected={getSelected} setTodoType={setTodoType} onHide={() => setModalDeleteShow(false)} show={modalDeleteShow} />
        <NewTodoTypeModalComponent setTodoType={setTodoType} onHide={() => setModalNewShow(false)} show={modalNewShow} />
      </Container>
    );
  }
  