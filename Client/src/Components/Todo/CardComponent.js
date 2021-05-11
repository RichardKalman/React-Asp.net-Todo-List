import { Droppable } from 'react-beautiful-dnd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { Card } from 'react-bootstrap';
import PropTypes from 'prop-types';
import ItemComponent from './ItemComponent';

const CardComponent = ({ card, kulcs, todo }) => {

  const itemFunctions = {
    setModalDeleteShow: card.setModalDeleteShow,
    setSelectedItem: card.setSelectedItem,
  };

  return (
    <Card key={kulcs.toUpperCase()} className="bg-egyedi color mb-3">
      
      <Card.Header>
        {todo.name}
        <span className="badge badge-secondary">{todo.items.length}</span>
        <span className="float-right">
          <FontAwesomeIcon
            className="add-fa"
            icon={faPlus}
            onClick={() => {
              card.setModalShow(true);
              card.setMezoNev(kulcs);
            }}
          />
        </span>
      </Card.Header>

      <Droppable droppableId={kulcs}>
        {(provided) => (
          <Card.Body ref={provided.innerRef} {...provided.droppableProps}>
            {todo.items.map((item, index) => (
              <ItemComponent functions={itemFunctions} key={'ic'+item.id} item={item} index={index} />
            ))}
            {provided.placeholder}
          </Card.Body>
        )}
      </Droppable>
    </Card>

  );
};
CardComponent.propTypes = {
  card: PropTypes.object.isRequired,
  kulcs: PropTypes.string.isRequired,
  // eslint-disable-next-line react/forbid-prop-types
  todo: PropTypes.object.isRequired,

};

export default CardComponent;