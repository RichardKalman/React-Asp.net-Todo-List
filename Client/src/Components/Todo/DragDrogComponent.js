import { useState, useEffect } from 'react';
import {Col, Container, Row, Card} from 'react-bootstrap';
import { toast } from 'react-toastify';
import { DragDropContext } from 'react-beautiful-dnd';
import _ from 'lodash';
import NewTodoModalComponent from './NewTodoModalComponent';
import CardKomponent from './CardComponent';

import DeleteTodoModalComponent from './DeleteTodoModalComponent';

const DragDropComponent = () => {
  const [getTodos, setTodos] = useState([]);

  const [existTable, setExistTable] = useState(true);
  const [modalShow, setModalShow] = useState(false);
  const [modalDeleteShow, setModalDeleteShow] = useState(false);
  const [selectedItem, setSelectedItem] = useState({});
  const [mezoName, setMezoName] = useState('');

  const allTodoLoad = async () => {
    let types = await fetch(`${process.env.REACT_APP_API_URL}/api/todotype`).then((res) => res.json());
    types = types.map((tt) => ({ ...tt, items: [] }))
      .reduce((aggregalt, tt) => ({ ...aggregalt, [tt.name.toString().toLowerCase()]: tt }), {});

    if (!_.isEmpty(types)) {
      const todos = await fetch(`${process.env.REACT_APP_API_URL}/api/todo`).then((res) => res.json());
      todos.forEach((data) => {
        types[data.type.name.toLowerCase()].items.push(data);
      });
      setExistTable(true);
    } else {
      setExistTable(false);
    }
    setTodos(types);
    return types;
  };

  useEffect(() => {
    allTodoLoad();
  }, []);

  const sendUpdateColumn = async (item, destinationid, destinationindex) => {
    const data = {
      "id": item.id,
      "Destinationtablename": destinationid,
      "destinationindex": destinationindex,
    };
    console.log(JSON.stringify(data))
    const requestOptions = {
      method: 'PUT',
      body: JSON.stringify(data),
      headers: {
        'Content-Type': 'application/json'
      },
    };
    try {
      fetch(
        `${process.env.REACT_APP_API_URL}/api/todo/toothercolumn`, requestOptions,
      );
    } catch {
      toast('Nem sikerült');
    }
  };

  const sendUpdateSort = async (item, destinationindex) => {
    const send = {
      "id": item.id,
      "destinationindex": destinationindex,
    };


    const requestOptions = {
      method: 'PUT',
      body: JSON.stringify(send),
      headers: {
        'Content-Type': 'application/json'
      },
    };
    try {
      fetch(
        `${process.env.REACT_APP_API_URL}/api/todo/sort`, requestOptions,
      );
    } catch (error) {
      toast('Nem sikerült');
    }
  };

  const handleDragEnd = ({ destination, source }) => {
    if (!destination) {
      return;
    }
    if (destination.index === source.index && destination.droppableId === source.droppableId) {
      return;
    }
    const itemCopy = { ...getTodos[source.droppableId].items[source.index] };
    // fel - le mozgásért felel
    if (destination.droppableId === source.droppableId && destination.index !== source.index) {
      sendUpdateSort(itemCopy, destination.index);
    }
    // oszlopok közötti mozgás
    if (destination.droppableId !== source.droppableId) {
      sendUpdateColumn(itemCopy, destination.droppableId, destination.index);
    }

    setTodos((prev) => {
      prev = { ...prev };
      prev[source.droppableId].items.splice(source.index, 1);

      prev[destination.droppableId].items.splice(destination.index, 0, itemCopy);

      return prev;
    });
  };
  const card = {
    setMezoNev: setMezoName,
    setModalShow,
    setModalDeleteShow,
    setSelectedItem,
  };

  if (existTable) {
    return (
      <Container>
        <Row>
          <DragDropContext onDragEnd={handleDragEnd}>

            {_.map(getTodos, (data, key) => (
              <Col lg={3} md={6} key={data.id.toString()}>
                <CardKomponent card={card} kulcs={key} todo={data} />
              </Col>
            ))}

          </DragDropContext>
        </Row>
        <NewTodoModalComponent setTodos={setTodos} mezo={mezoName} variant="primary" onHide={() => setModalShow(false)} show={modalShow} />
        <DeleteTodoModalComponent variant="primary" item={selectedItem} setTodos={setTodos} onHide={() => setModalDeleteShow(false)} show={modalDeleteShow} />
      </Container>

    );
  }

  return (
    <Container>
      <Row>
        <Col>
          <Card className="bg-dark color">
            <Card.Body className="p-3 ">
              <Card.Text>Nincsennek táblák a rendszerben, Kérlek adj hozzá először egyet. </Card.Text>

            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}


export default DragDropComponent;